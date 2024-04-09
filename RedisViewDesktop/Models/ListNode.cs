using ReactiveUI;

namespace RedisViewDesktop.Models
{
    public class ListNode : ReactiveObject
    {
        public string UID { get; }
        private string? data;
        public string? Data
        {
            get => data; set
            {
                this.RaiseAndSetIfChanged(ref data, value);
            }
        }

        public ListNode(string uid, string? data)
        {
            UID = uid;
            Data = data;
        }
    }
}
