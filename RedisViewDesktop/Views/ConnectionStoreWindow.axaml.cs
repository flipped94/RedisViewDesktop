using System;
using RedisViewDesktop.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;


namespace RedisViewDesktop.Views;

public partial class ConnectionWindow : ReactiveWindow<ConnectionStoreViewModel>
{
    public ConnectionWindow()
    {
        InitializeComponent();        
        this.WhenActivated(action =>
        {
            action(ViewModel!.AddConnectionCommand.Subscribe(connectionViewModel =>
            {
                if (connectionViewModel != null)
                {
                    Close(connectionViewModel);
                }
            }));
        });
    }
}