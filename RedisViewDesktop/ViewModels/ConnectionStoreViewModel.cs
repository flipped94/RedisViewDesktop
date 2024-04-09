using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using StackExchange.Redis;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;

namespace RedisViewDesktop.ViewModels
{
    public class ConnectionStoreViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, ConnectionViewModel?> AddConnectionCommand { get; }
        public ReactiveCommand<Unit, Unit> TestConnectionCommand { get; }

        private string name = "127.0.0.1:6379";
        public string Name
        {
            get => name;
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        private string usertname = "";
        public string Username
        {
            get => usertname;
            set
            {
                this.RaiseAndSetIfChanged(ref usertname, value);
            }
        }

        private string hostname = "192.168.137.133";
        public string Hostname
        {
            get => hostname;
            set
            {
                this.RaiseAndSetIfChanged(ref hostname, value);
            }
        }

        private int port = 6379;
        public int Port
        {
            get => port;
            set
            {
                this.RaiseAndSetIfChanged(ref port, value);
            }
        }

        private string password = "";
        public string Password
        {
            get => password;
            set
            {
                this.RaiseAndSetIfChanged(ref password, value);
            }
        }

        private int db = 0;
        public int Db
        {
            get => db;
            set
            {
                this.RaiseAndSetIfChanged(ref db, value);
            }
        }

        public ObservableCollection<int> RDBS { get; } = [];

        private int timeoutSec = 30;
        public int TimeoutSec
        {
            get => timeoutSec;
            set
            {
                this.RaiseAndSetIfChanged(ref timeoutSec, value);
            }
        }

        private string delimiter = ":";
        public string Delimiter
        {
            get => delimiter;
            set
            {
                this.RaiseAndSetIfChanged(ref delimiter, value);
            }
        }

        public ConnectionStoreViewModel()
        {
            AddConnectionCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Connection connection = new(Name, Username, Password, Hostname, Port, Db, TimeoutSec, Delimiter);
                try
                {
                    await RedisHelper.ConnectAsync(connection);
                    return new ConnectionViewModel(connection);
                }
                catch (RedisConnectionException e)
                {
                    Debug.WriteLine($"Failed to connect connection: {e}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to connect connection: {e}");
                }
                return null;
            });

            TestConnectionCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Connection connection = new(Name, Username, Password, Hostname, Port, Db, TimeoutSec, Delimiter);
                try
                {
                    await RedisHelper.TestConnectAsync(connection);
                }
                catch (RedisConnectionException e)
                {
                    Debug.WriteLine($"Failed to connect connection: {e}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to connect connection: {e}");
                }
            });

            for (int i = 0; i < 16; i++)
            {
                RDBS.Add(i);
            }
        }
    }
}