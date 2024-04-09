using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.ViewModels;
using System;

namespace RedisViewDesktop.Views;

public partial class ListElementAddWindow : ReactiveWindow<ListElementAddViewModel>
{
    public ListElementAddWindow()
    {
        InitializeComponent();
        this.WhenActivated(action =>
        {
            action(ViewModel!.CancelCommand.Subscribe(Close));
            action(ViewModel!.SaveCommand.Subscribe(Close));
        });
    }
}