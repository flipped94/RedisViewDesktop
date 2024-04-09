using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace RedisViewDesktop.Helpers
{
    public class SqlliteHelper
    {
        private static DbCommand Command;

        private SqlliteHelper() { }

        public static DbCommand CreateCommand()
        {
            if (Command == null)
            {
                DbConnection connection = new SqliteConnection("Data Source=Connections.db;");
                connection.Open();
                Command = connection.CreateCommand();
                return Command;
            }
            return Command;
        }

    }
}
