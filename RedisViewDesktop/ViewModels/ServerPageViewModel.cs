using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace RedisViewDesktop.ViewModels
{
    public class ServerPageViewModel : ViewModelBase
    {
        public ObservableCollection<ServerField> ServerFields { get; } = [];

        public ServerPageViewModel()
        {
            RxApp.MainThreadScheduler.Schedule(async () =>
            {
                var connection = await Connection.GetLastConnectedConnectionAsync();
                await RedisHelper.ConnectAsync(connection!);
                await PrepareData("SERVER");
            });
        }

        public async Task PrepareData(string? header)
        {
            ServerFields.Clear();
            if (header is null)
            {
                header = "MEMORY";
            }
            if (header is not null)
            {
                ExecuteResult res = await RedisHelper.ExecuteAsync("INFO", [header]);
                if (res is not null && res.Success)
                {
                    object ms = res.Message;
                    if (ms is not null && ms is string m)
                    {
                        string[] fields = m.Split("\r\n");
                        if (fields is not null)
                        {
                            for (int i = 0; i < fields.Length; i++)
                            {
                                string field = fields[i];
                                if (i == 0 || string.IsNullOrEmpty(field))
                                {
                                    continue;
                                }
                                var index = field.IndexOf(':');
                                if (index >= 0)
                                {
                                    ServerFields.Add(new ServerField(field[..(index)], field[(index + 1)..]));
                                }
                                else
                                {
                                    ServerFields.Add(new ServerField(fields[i], ""));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}