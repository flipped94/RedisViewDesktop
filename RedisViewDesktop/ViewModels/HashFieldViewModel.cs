using ReactiveUI;

namespace RedisViewDesktop.ViewModels
{
    public class HashFieldViewModel : ViewModelBase
    {

        public string Key { get; set; }
        public string Field { get; set; }
        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
            }
        }

        public HashFieldViewModel(string key, string field, string value)
        {
            Key = key;
            Field = field;
            Value = value;
        }

    }
}
