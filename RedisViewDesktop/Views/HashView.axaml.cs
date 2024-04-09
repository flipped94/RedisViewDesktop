using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisViewDesktop.Views;

public partial class HashView : ReactiveUserControl<HashViewModel>
{
    public HashView()
    {
        InitializeComponent();
        this.WhenActivated((x) =>
        {
            HashViewModel hashViewModel = (HashViewModel)this.DataContext;
            hashViewModel.Ready();
            ViewModel!.ShowEditHashFieldDialog.RegisterHandler(DoShowEditMemberDialogAsync);
            ViewModel!.ShowAddFieldsDialog.RegisterHandler(DoShowAddFieldsDialogAsync);
        });
    }

    private async Task DoShowAddFieldsDialogAsync(InteractionContext<HashFieldAddViewModel, Dictionary<string, string>> interaction)
    {
        HashFieldAddWindow dialog = new HashFieldAddWindow();
        Window? parent = WindowHelper.FindParentWindow(this);
        dialog.DataContext = interaction.Input;
        if (parent != null)
        {
            var result = await dialog.ShowDialog<Dictionary<string, string>>(parent);
            if (result != null)
            {
                interaction.SetOutput(result);
            }
        }
    }

    private async Task DoShowEditMemberDialogAsync(InteractionContext<HashFieldViewModel, HashFieldViewModel?> interaction)
    {
        HashFieldEditWindow dialog = new HashFieldEditWindow();
        HashFieldViewModel hashField = interaction.Input;
        HashFieldEditViewModel dataContext = new HashFieldEditViewModel();
        dataContext.Key = hashField.Key;
        dataContext.Field = hashField.Field;
        dataContext.Value = hashField.Value;
        dialog.DataContext = dataContext;
        Window? parent = WindowHelper.FindParentWindow(this);
        if (parent != null)
        {
            var result = await dialog.ShowDialog<HashFieldViewModel>(parent);
            interaction.SetOutput(result);
        }
    }
}