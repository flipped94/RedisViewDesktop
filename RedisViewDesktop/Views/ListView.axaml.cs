using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using RedisViewDesktop.ViewModels;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace RedisViewDesktop.Views;

public partial class ListView : ReactiveUserControl<ListViewModel>
{
    public ListView()
    {
        InitializeComponent();
        this.WhenActivated((x) =>
        {
            ListViewModel listViewModel = (ListViewModel)this.DataContext;
            listViewModel.Ready();
            ViewModel!.ShowEditListElementDialog.RegisterHandler(DoShowEditElementDialogAsync);
            ViewModel!.ShowAddListElementsDialog.RegisterHandler(DoShowAddElementsDialogAsync);
        });
    }

    private async Task DoShowAddElementsDialogAsync(InteractionContext<Unit, List<ListNode>> interaction)
    {
        ListElementAddWindow dialog = new ListElementAddWindow();
        Window? parent = WindowHelper.FindParentWindow(this);
        dialog.DataContext = new ListElementAddViewModel();
        if (parent != null)
        {
            var result = await dialog.ShowDialog<List<ListNode>>(parent);
            if (result != null)
            {
                interaction.SetOutput(result);
            }
        }
    }

    private async Task DoShowEditElementDialogAsync(InteractionContext<string, string?> interaction)
    {
        ListElementEditWindow dialog = new();

        string data = interaction.Input;
        ListElementEditViewModel dataContext = new()
        {
            Element = data,
        };
        dialog.DataContext = dataContext;
        Window? parent = WindowHelper.FindParentWindow(this);
        if (parent != null)
        {
            var result = await dialog.ShowDialog<string>(parent);
            interaction.SetOutput(result);
        }
    }
}