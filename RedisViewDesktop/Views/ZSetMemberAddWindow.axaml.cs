using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.ViewModels;
using System;

namespace RedisViewDesktop.Views;

public partial class ZSetMemberAddWindow : ReactiveWindow<ZSetMemberAddViewModel>
{
    public ZSetMemberAddWindow()
    {
        InitializeComponent();
        this.WhenActivated(action =>
        {
            action(ViewModel!.CancelCommand.Subscribe(Close));
            action(ViewModel!.SaveCommand.Subscribe(Close));
        });
    }
}