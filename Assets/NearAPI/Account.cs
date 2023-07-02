using System;
using System.Text;
using System.Threading.Tasks;
using NearClientUnity.Utilities;
using Newtonsoft.Json;

namespace Near
{
    public class Account
    {
        private const ulong DEFAULT_GAS = 300000000000000;
        private const ushort DEFAULT_DEPOSIT = 0;
        private Provider _provider;
        private Signer _signer;
        private AccessKey _accessKey = null;
        private string _accountId;

        public Account(string ctrId, string accId, string privKey)
        {
            _provider = new JsonRpcProvider();
            _signer = new InMemorySigner(ctrId, accId, privKey);
            _accountId = accId;
        }

        public async Task<string> FunctionViewAsync(string method, object args = null)
        {
            var jsonArgs = JsonConvert.SerializeObject(args ?? new { });
            var data = Base58.Encode(Encoding.UTF8.GetBytes(jsonArgs));
            return await _provider.QueryAsync($"call/{_accountId}/{method}", data);
        }

        public async Task<FinalExecutionOutcome> FunctionCallAsync(string method, object args = null, ulong? gas = null, Nullable<UInt128> amount = null)
        {
            var jsonArgs = JsonConvert.SerializeObject(args ?? new { });
            var data = Encoding.UTF8.GetBytes(jsonArgs);
            var act = new Action
            {
                type = ActionType.FunctionCall,
                args = new FunctionCallActionArgs
                {
                    method = method,
                    args = data,
                    gas = gas ?? DEFAULT_GAS,
                    deposit = amount ?? DEFAULT_DEPOSIT
                }
            };

            return await SignAndSendTransactionAsync(new Action[] { act });
        }

        private async Task<FinalExecutionOutcome> SignAndSendTransactionAsync(Action[] acts)
        {
            await GetReadyAsync();

            var status = await _provider.GetStatusAsync();
            var (txHash, tx) = _signer.SignTransaction(++_accessKey.nonce, acts, new ByteArray32
            {
                Buffer = Base58.Decode(status.sync_info.latest_block_hash)
            });

            var res = await _provider.SendTransactionAsync(tx);
            if (res.status != null && res.status.Failure != null)
            {
                throw new Exception(res.status.GetFailure());
            }

            return res;
        }

        private async Task GetReadyAsync()
        {
            if (_accessKey == null)
            {
                try
                {
                    var pubKey = _signer.GetPublicKey();
                    var res = await _provider.QueryAsync($"access_key/{_accountId}/{pubKey.ToString()}", "");
                    _accessKey = JsonConvert.DeserializeObject<AccessKey>(res);
                }
                catch (Exception err)
                {
                    _accessKey = null;
                    throw err;
                }
            }
        }
    }
}
