using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisViewDesktop.Views;

public partial class ZSetView : ReactiveUserControl<ZSetViewModel>
{
    public ZSetView()
    {
        InitializeComponent();
        this.WhenActivated((x) =>
        {
            ZSetViewModel zSetViewModel = (ZSetViewModel)this.DataContext;
            zSetViewModel.Ready();
            ViewModel!.ShowEditSortedSetMemberDialog.RegisterHandler(DoShowEditMemberDialogAsync);
            ViewModel!.ShowAddMembersDialog.RegisterHandler(DoShowAddMembersDialogAsync);
        });
    }

    private async Task DoShowAddMembersDialogAsync(InteractionContext<ZSetMemberAddViewModel, Dictionary<string, double>> interaction)
    {
        ZSetMemberAddWindow dialog = new();
        Window? parent = WindowHelper.FindParentWindow(this);
        dialog.DataContext = interaction.Input;
        if (parent != null)
        {
            var result = await dialog.ShowDialog<Dictionary<string, double>>(parent);
            if (result != null)
            {
                interaction.SetOutput(result);
            }
        }
    }

    private async Task DoShowEditMemberDialogAsync(InteractionContext<ZSetMemberViewModel, ZSetMemberViewModel?> interaction)
    {
        ZSetMemberEditWindow dialog = new();
        ZSetMemberViewModel zSetMember = interaction.Input;
        ZSetMemberEditViewModel dataContext = new()
        {
            Key = zSetMember.Key,
            Member = zSetMember.Member,
            Score = zSetMember.Score + ""
        };
        dialog.DataContext = dataContext;
        Window? parent = WindowHelper.FindParentWindow(this);
        if (parent != null)
        {
            var result = await dialog.ShowDialog<ZSetMemberViewModel>(parent);
            interaction.SetOutput(result);
        }
    }
}