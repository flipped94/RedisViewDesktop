using ReactiveUI;

namespace RedisViewDesktop.ViewModels
{
    public class ZSetMemberViewModel : ViewModelBase
    {

        public string Key { get; set; }
        public string Member { get; set; }
        private double score;
        public double Score
        {
            get => score;
            set
            {
                this.RaiseAndSetIfChanged(ref score, value);
            }
        }

        public ZSetMemberViewModel(string key, string member, double score)
        {
            Key = key;
            Member = member;
            Score = score;
        }

    }
}
