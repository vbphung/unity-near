using System.Threading.Tasks;

namespace Near
{
    public abstract class Provider
    {
        public abstract Task<NodeStatusResult> GetStatusAsync();
        public abstract Task<string> QueryAsync(string path, string data);
        public abstract Task<FinalExecutionOutcome> SendTransactionAsync(SignedTransaction signedTx);
    }
}
