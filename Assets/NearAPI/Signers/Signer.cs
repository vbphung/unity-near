using System;
using System.Security.Cryptography;
using NearClientUnity.Utilities;

namespace Near
{
    public abstract class Signer
    {
        private string _contractId, _accountId;

        public Signer(string ctrId, string accId)
        {
            _contractId = ctrId;
            _accountId = accId;
        }

        public abstract PublicKey GetPublicKey();
        public abstract Signature SignHash(byte[] hash);

        public Tuple<byte[], SignedTransaction> SignTransaction(ulong nonce, Action[] actions, ByteArray32 blockHash)
        {
            var pubKey = GetPublicKey();
            var tx = new Transaction
            {
                signer_id = _accountId,
                public_key = pubKey,
                nonce = nonce,
                receiver_id = _contractId,
                actions = actions,
                block_hash = blockHash
            };

            var msg = tx.ToByteArray();

            var hash = new byte[] { };
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(msg);
            }

            var sign = SignMessage(msg);
            var signedTx = new SignedTransaction
            {
                transaction = tx,
                signature = new NearSignature
                {
                    data = new ByteArray64
                    {
                        Buffer = sign.SignatureBytes
                    }
                }
            };

            return new Tuple<byte[], SignedTransaction>(hash, signedTx);
        }

        public Signature SignMessage(byte[] msg)
        {
            var msgSha256 = new byte[] { };
            using (var sha256 = SHA256.Create())
            {
                msgSha256 = sha256.ComputeHash(msg);
            }

            return SignHash(msgSha256);
        }
    }
}
