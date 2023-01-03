using System;
using System.IO;
using NearClientUnity.Utilities;

namespace Near
{
    [System.Serializable]
    public class SignedTransaction : IByteArrayData
    {
        public NearSignature signature;
        public Transaction transaction;

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var wr = new NearBinaryWriter(ms))
                {
                    wr.Write(transaction.ToByteArray());
                    wr.Write(signature.ToByteArray());
                    return ms.ToArray();
                }
            }
        }

        public string ToBase64String()
        {
            var data = ToByteArray();
            return Convert.ToBase64String(data);
        }
    }

    [System.Serializable]
    public class NearSignature : IByteArrayData
    {
        public ByteArray64 data;

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var wr = new NearBinaryWriter(ms))
                {
                    wr.Write((byte)KeyType.Ed25519);
                    wr.Write(data.Buffer);
                    return ms.ToArray();
                }
            }
        }
    }

    [System.Serializable]
    public class Transaction : IByteArrayData
    {
        public Action[] actions;
        public ByteArray32 block_hash;
        public ulong nonce;
        public PublicKey public_key;
        public string receiver_id, signer_id;

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var wr = new NearBinaryWriter(ms))
                {
                    wr.Write(signer_id);
                    wr.Write(public_key.ToByteArray());
                    wr.Write(nonce);
                    wr.Write(receiver_id);
                    wr.Write(block_hash.Buffer);

                    wr.Write((uint)actions.Length);
                    foreach (var act in actions)
                    {
                        wr.Write(act.ToByteArray());
                    }

                    return ms.ToArray();
                }
            }
        }
    }
}
