using System.IO;
using NearClientUnity.Utilities;
using Newtonsoft.Json;

namespace Near
{
    [System.Serializable]
    public class Action : IByteArrayData
    {
        public object args;
        public ActionType type;

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var wr = new NearBinaryWriter(ms))
                {
                    wr.Write((byte)type);

                    var funcCallArgs = GetArgs<FunctionCallActionArgs>();
                    wr.Write(funcCallArgs.method);
                    wr.Write((uint)funcCallArgs.args.Length);
                    wr.Write(funcCallArgs.args);
                    wr.Write((ulong)funcCallArgs.gas);
                    wr.Write((UInt128)funcCallArgs.deposit);

                    return ms.ToArray();
                }
            }
        }

        private T GetArgs<T>()
        {
            var jsonArgs = JsonConvert.SerializeObject(args);
            return JsonConvert.DeserializeObject<T>(jsonArgs);
        }
    }

    public enum ActionType
    {
        CreateAccount = 0,
        DeployContract,
        FunctionCall,
        Transfer,
        Stake,
        AddKey,
        DeleteKey,
        DeleteAccount
    }

    [System.Serializable]
    public class FunctionCallActionArgs
    {
        public string method;
        public byte[] args;
        public ulong? gas;
        public UInt128 deposit;
    }
}
