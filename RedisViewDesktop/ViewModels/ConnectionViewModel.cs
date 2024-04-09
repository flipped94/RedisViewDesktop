using System.Threading.Tasks;
using Avalonia.Media;
using Connection = RedisViewDesktop.Models.Connection;

namespace RedisViewDesktop.ViewModels
{
    public class ConnectionViewModel(Connection connection) : ViewModelBase
    {
        private readonly Connection connection = connection;

        public string Name => connection.Name;

        public string Alias => connection.Alias;

        public string ServerType => connection.ServerType;

        public Color ColorStart => connection.ColorStart;

        public Color ColorStop => connection.ColorStop;

        public string Hostname => connection.Host + ":" + connection.Port;

        public int Id => connection.Id;

        public string LastConnectedTime => connection.LastConnectedTime;

        public static async void UpdateLastConnection(int id)
        {
            await Connection.UpdateLastConnectedTimeAsync(id);
        }

        public async Task SaveToDB()
        {
            await connection.SaveAsync();
        }
    }
}