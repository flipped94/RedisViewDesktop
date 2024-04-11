using AvaloniaEdit.Utils;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{
    public class PaneCliViewModel : ViewModelBase
    {

        public ObservableCollection<string> Hints { get; } = [];

        private string? commandAndArgs;
        public string? CommandAndArgs
        {
            get => commandAndArgs; set
            {
                this.RaiseAndSetIfChanged(ref commandAndArgs, value);
                DoTip();
            }
        }

        private string? selectedCommand;
        public string? SelectedCommand
        {
            get => selectedCommand;
            set
            {
                if (null != value)
                {
                    CommandAndArgs = value;
                    Hints.Clear();
                }
                this.RaiseAndSetIfChanged(ref selectedCommand, value);
            }
        }

        public ObservableCollection<ExecuteResult> Results { get; } = [];

        public ICommand ExecuteCommand { get; }

        public PaneCliViewModel()
        {
            ExecuteCommand = ReactiveCommand.Create(async () =>
            {
                await Exexute();
                CommandAndArgs = "";
            });
        }

        private async Task Exexute()
        {
            if (string.IsNullOrEmpty(CommandAndArgs))
            {
                return;
            }
            var cmas = CommandAndArgs.Split(" ");
            ExecuteResult executeResult;
            if (cmas.Length == 1)
            {
                executeResult = await RedisHelper.ExecuteAsync(cmas[0], null);
            }
            else
            {
                object[] args = new object[cmas.Length - 1];
                for (int i = 1; i < cmas.Length; i++)
                {
                    args[i - 1] = cmas[i];
                }
                executeResult = await RedisHelper.ExecuteAsync(cmas[0], args);
            }
            if (null == executeResult)
            {
                return;
            }

            var c = string.Join("\t", cmas);
            executeResult.Message = $"Command:\t{c}\r\n" + executeResult.Message;
            Results.Add(executeResult);
        }

        private void DoTip()
        {
            if (!string.IsNullOrEmpty(CommandAndArgs) && !string.IsNullOrEmpty(CommandAndArgs.Trim()))
            {
                List<string> commands = CommandHelper.GetTips(CommandAndArgs);
                Hints.Clear();
                Hints.AddRange(commands);
            }
            else
            {
                Hints.Clear();
            }
        }
    }
}