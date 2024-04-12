using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{
    public class PaneLuaViewModel : ViewModelBase
    {
        public ExecuteResult? result;
        public ExecuteResult? Result
        {
            get => result;
            set
            {
                this.RaiseAndSetIfChanged(ref result, value);
            }
        }

        public ObservableCollection<Node> Keys { get; } = [];

        private Node selectedKey;
        public Node SelectedKey
        {
            get => selectedKey;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedKey, value);
            }
        }

        public ObservableCollection<Node> Values { get; } = [];
        private Node selectedValue;
        public Node SelectedValue
        {
            get => selectedValue;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedValue, value);
            }
        }

        public ICommand AddKeyCommand { get; }
        public ICommand RemoveKeyCommand { get; }

        public ICommand AddValueCommand { get; }
        public ICommand RemoveValueCommand { get; }

        public PaneLuaViewModel()
        {
            AddKeyCommand = ReactiveCommand.Create(() =>
            {
                Keys.Add(new Node("", 1));
            });

            RemoveKeyCommand = ReactiveCommand.Create(() =>
            {
                Keys.Remove(SelectedKey);
            });

            AddValueCommand = ReactiveCommand.Create(() =>
            {
                Values.Add(new Node("", 1));
            });

            RemoveValueCommand = ReactiveCommand.Create(() =>
            {
                Values.Remove(SelectedValue);
            });

        }

        public async Task ExecuteScript(string? script)
        {
            if (!string.IsNullOrEmpty(script))
            {
                Result = await RedisHelper.ScriptEvaluateAsync(script, Keys.Select(x => x.Element).ToList(), Values.Select(x => x.Element).ToList());
            }
        }
    }
}