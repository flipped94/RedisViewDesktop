using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.ViewModels;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace RedisViewDesktop.Views;

public partial class SetView : ReactiveUserControl<SetViewModel>
{
    public SetView()
    {
        InitializeComponent();
        this.WhenActivated((x) =>
        {
            SetViewModel setViewModel = (SetViewModel)this.DataContext;
            setViewModel.Ready();
            ViewModel!.ShowAddSetMemberDialog.RegisterHandler(DoShowAddMembersDialogAsync);
            ViewModel!.ShowEditSetMemberDialog.RegisterHandler(DoShowEditMemberDialogAsync);
        });
        
    }

    private async Task DoShowAddMembersDialogAsync(InteractionContext<Unit, HashSet<string>> interaction)
    {
        SetMemberAddWindow dialog = new();
        Window? parent = WindowHelper.FindParentWindow(this);

        dialog.DataContext = new SetMemberAddViewModel();
        if (parent != null)
        {
            var result = await dialog.ShowDialog<HashSet<string>>(parent);
            if (result != null)
            {
                interaction.SetOutput(result);
            }
        }
    }

    private async Task DoShowEditMemberDialogAsync(InteractionContext<string?, string?> interaction)
    {
        SetMemberEditWindow dialog = new();
        SetMemberEditViewModel dataContext = new()
        {
            Member = interaction.Input
        };
        dialog.DataContext = dataContext;
        Window? parent = WindowHelper.FindParentWindow(this);
        if (parent != null)
        {
            var result = await dialog.ShowDialog<string?>(parent);
            interaction.SetOutput(result);            
        }
    }

}