using ReactiveUI;
using System.Reactive;

namespace RedisViewDesktop.ViewModels
{
    public class SetMemberEditViewModel : ReactiveObject
    {      
        public string? Member { get; set; }

        public ReactiveCommand<Unit, string?> EditCommand { get; set; }
        public ReactiveCommand<Unit, string?> CancelCommand { get; set; }

        public SetMemberEditViewModel()
        {
            EditCommand = ReactiveCommand.Create(() =>
            {
                return Member;
            });
            CancelCommand = ReactiveCommand.Create(() =>
            {
                return Member;
            });

        }
    }
}