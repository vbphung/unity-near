using Newtonsoft.Json;

namespace Near
{
    [System.Serializable]
    public class FinalExecutionOutcome
    {
        public FinalExecutionStatus status;
    }

    [System.Serializable]
    public class FinalExecutionStatus
    {
        public object Failure;
        public string SuccessValue;

        public string GetFailure()
        {
            var json = JsonConvert.SerializeObject(Failure);
            return JsonConvert.DeserializeAnonymousType(json, new
            {
                ActionError = new
                {
                    index = int.MinValue,
                    kind = new
                    {
                        FunctionCallError = new
                        {
                            ExecutionError = string.Empty
                        }
                    }
                }
            }).ActionError.kind.FunctionCallError.ExecutionError;
        }
    }
}
