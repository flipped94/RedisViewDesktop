using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.ViewModels;
using System;

namespace RedisViewDesktop.Views;

public partial class HashFieldEditWindow : ReactiveWindow<HashFieldEditViewModel>
{
    public HashFieldEditWindow()
    {
        InitializeComponent();
        this.WhenActivated(action =>
        {
            // edit member commands
            action(ViewModel!.EditCommand.Subscribe(Close));
            action(ViewModel!.CancelCommand.Subscribe(Close));
        });
    }
}