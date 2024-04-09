namespace RedisViewDesktop.Models
{
    public class ExecuteResult
    {
        public bool Success { get; set; }
        public object Message { get; set; }

        public ExecuteResult(bool success, object message)
        {
            Success = success;
            Message = message;
        }
    }
}
