using ReactiveUI;
using RedisViewDesktop.Helpers;
using System.Reactive;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{
    public class KeyViewModelBase : ViewModelBase
    {
        public string Key { get; set; } = "";
        public string Type { get; set; } = "";
        public string Color { get; set; } = "";
        public KeysPageViewModel? KeysPage { get; set; }

        private string memoryUsage;
        public string MemoryUsage
        {
            get => memoryUsage; set
            {
                this.RaiseAndSetIfChanged(ref memoryUsage, value);
            }
        }

        private string length;
        public string Length
        {
            get => length; set
            {
                this.RaiseAndSetIfChanged(ref length, value);
            }
        }
        private string ttl;
        public string TTL
        {
            get => ttl; set
            {
                this.RaiseAndSetIfChanged(ref ttl, value);
            }
        }

        public ICommand DeleteCommand { get; }

        public KeyViewModelBase() 
        {
            DeleteCommand = ReactiveCommand.Create( async () =>
            {
                if(KeysPage is not null)
                {
                    await KeysPage.DeleteKey();
                }
            });
        }
    }
}
