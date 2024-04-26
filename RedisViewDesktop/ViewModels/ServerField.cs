using ReactiveUI;

namespace RedisViewDesktop.ViewModels
{
    public class ServerField : ReactiveObject
    {
        public string Field { get; }
        public string Value { get; }

        public ServerField(string field, string value)
        {
            Field = field;
            Value = value;
        }
    }
}
