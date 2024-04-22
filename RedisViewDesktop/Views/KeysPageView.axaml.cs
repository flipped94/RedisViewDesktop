using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using RedisViewDesktop.ViewModels;
using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Timers;

namespace RedisViewDesktop.Views;

public partial class KeysPageView : ReactiveUserControl<KeysPageViewModel>
{
    private Timer timer;

    public KeysPageView()
    {
        InitializeComponent();
        this.WhenActivated(action => action(ViewModel!.AddNewKeyInteraction.RegisterHandler(DoShowAddNewKeyDialog)));
    }

    private async Task DoShowAddNewKeyDialog(InteractionContext<NewKeyViewModel, AddKey?> interaction)
    {
        NewKeyWindow dialog = new NewKeyWindow();
        dialog.DataContext = interaction.Input;
        var newKey = await dialog.ShowDialog<AddKey?>(WindowHelper.FindParentWindow(this)!);
        interaction.SetOutput(newKey);
    }

    private void ScannedKeys_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        KeysScrollViewer.ScrollToEnd();
    }   

    private void EditRefreshRate(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is not null)
        {
            ViewModel.IsEditAutoRefresh = true;
            if (EditRefeshTextBox.Text is not null && EditRefeshTextBox.Text.Length > 0)
            {
                EditRefeshTextBox.Focus();
                EditRefeshTextBox.CaretIndex = EditRefeshTextBox.Text.Length;
            }
        }
    }

    private void EditCancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CancelEdit();
    }

    private void CancelEdit()
    {
        if (ViewModel is not null)
        {
            ViewModel.IsEditAutoRefresh = false;
        }
    }

    private void EditCheck(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is not null)
        {
            if (int.TryParse(EditRefeshTextBox.Text, out int interval))
            {
                ViewModel.IsEditAutoRefresh = false;
                ViewModel.RefreshRate = interval <= 0 ? 30 : interval;
                StartTimer();
            }
        }
    }


    private void Flyout_Closed(object? sender, System.EventArgs e)
    {
        CancelEdit();
    }

    private void StartTimer()
    {
        if (ViewModel is not null)
        {
            if (ViewModel.IsAutoRefresh)
            {
                ClearTimer();
                timer = new Timer(ViewModel.RefreshRate * 1000);
                timer.Elapsed += AutoRefresh;
                timer.Start();
            }
        }
    }

    private void AutoRefresh(object? sender, ElapsedEventArgs e)
    {
        RxApp.MainThreadScheduler.Schedule(async () =>
        {
            await ViewModel!.RefreshInterval();
        });
    }

    private void ClearTimer()
    {
        if (timer is not null)
        {
            timer.Stop();
            timer.Elapsed -= AutoRefresh;
            timer.Dispose();
        }
    }

    private void ToggleButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ToggleButton? toggleButton = sender as ToggleButton;
        if (toggleButton is not null&&ViewModel is not null)
        {
            if (toggleButton.IsChecked is not null && (bool)toggleButton.IsChecked)
            {
                ViewModel.IsAutoRefresh = true;
                StartTimer();
            }
            else
            {
                ClearTimer();
            }
        }
    }
}