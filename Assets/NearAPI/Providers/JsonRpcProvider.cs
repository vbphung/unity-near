using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Near
{
    public class JsonRpcProvider : Provider
    {
        private const string TESTNET_URL = "https://rpc.testnet.near.org";
        private const string MAINNET_URL = "https://rpc.mainnet.near.org";
        private uint _id = 0;

        public override async Task<NodeStatusResult> GetStatusAsync()
        {
            var res = await SendJsonRpc("status");
            return JsonConvert.DeserializeObject<NodeStatusResult>(res);
        }

        public override async Task<string> QueryAsync(string path, string data = null)
        {
            var paramArr = new object[] { path, data ?? string.Empty };
            return await SendJsonRpc("query", paramArr);
        }

        public override async Task<FinalExecutionOutcome> SendTransactionAsync(SignedTransaction signedTx)
        {
            var txBase64 = signedTx.ToBase64String();
            var paramArr = new object[] { txBase64 };

            var res = await SendJsonRpc("broadcast_tx_commit", paramArr);

            return JsonConvert.DeserializeObject<FinalExecutionOutcome>(res);
        }

        private async Task<string> SendJsonRpc(string method, object[] paramArr = null)
        {
            var req = new UnityWebRequest(TESTNET_URL, "POST");
            req.SetRequestHeader("Content-Type", "application/json");
            req.downloadHandler = new DownloadHandlerBuffer();

            var data = RequestData(method, paramArr);
            req.uploadHandler = new UploadHandlerRaw(data);

            var resTxt = await SendRequest(req);
            var res = JsonConvert.DeserializeAnonymousType(resTxt, new
            {
                result = new object()
            }).result;

            return JsonConvert.SerializeObject(res);
        }

        private async Task<string> SendRequest(UnityWebRequest req)
        {
            var send = req.SendWebRequest();
            while (!send.isDone)
            {
                await Task.Delay(1000 / 24);
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new System.Exception(req.error);
            }

            return req.downloadHandler.text;
        }

        private byte[] RequestData(string method, object[] paramArr = null)
        {
            paramArr ??= new object[] { };
            var body = new
            {
                method,
                paramArr,
                jsonrpc = "2.0",
                id = _id++
            };

            var jsonBody = JsonConvert.SerializeObject(body);
            jsonBody = jsonBody.Replace("paramArr", "params");

            return Encoding.UTF8.GetBytes(jsonBody);
        }
    }
}
