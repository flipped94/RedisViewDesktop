using Avalonia.Media;
using RedisViewDesktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace RedisViewDesktop.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Username { get; set; }
        public Color ColorStart { get; set; }
        public Color ColorStop { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int Db { get; set; }
        public int TimeoutSec { get; set; }
        public string ServerType { get; set; }
        public string LastConnectedTime { get; set; }
        public string Delimiter { get; set; }

        public Connection(Builder builder)
        {
            Name = builder.name;
            Alias = builder.alias;
            Username = builder.username;
            Password = builder.password;
            Host = builder.host;
            Port = builder.port;
            Db = builder.db;
            TimeoutSec = builder.timeoutSec;
            Delimiter = builder.delimiter;
        }

        public class Builder(string name, string host, int port)
        {
            public string name { get; set; } = name;
            public string alias { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string host { get; set; } = host;
            public int port { get; set; } = port;
            public int db { get; set; }
            public int timeoutSec { get; set; }
            public string delimiter { get; set; }

            public Builder Username(string username)
            {
                this.username = username;
                return this;
            }

            public Builder Alias(string alias)
            {
                this.alias = alias;
                return this;
            }

            public Builder Password(string password)
            {
                this.password = password;
                return this;
            }

            public Builder Port(int port)
            {
                this.port = port;
                return this;
            }

            public Builder Db(int db)
            {
                this.db = db;
                return this;
            }

            public Builder TimeoutSec(int timeoutSec)
            {
                this.timeoutSec = timeoutSec;
                return this;
            }

            public Builder Delimiter(string delimiter)
            {
                this.delimiter = delimiter;
                return this;
            }

            public Connection Build()
            {
                return new Connection(this);
            }
        }

        public Connection(string name, string username, string password, string host, int port, int db, int timeoutSec, string delimiter)
        {
            Name = name;
            Alias = name;
            Username = username;
            Password = password;
            Host = host;
            Port = port;
            Db = db;
            TimeoutSec = timeoutSec;
            GradientColor gradientColor = GradientHelper.GradientColor(new Random().Next(0, GradientHelper.Colors.Length));
            ColorStart = gradientColor.ColorStart;
            ColorStop = gradientColor.ColorStop;
            ServerType = "";
            LastConnectedTime = "";
            Delimiter = delimiter;
        }

        private Connection(int id, string name, string alias, string username, string password,
            string host, int port, int db, int timeoutSec, Color colotStart, Color colorEnd, string serverType, string lastConnectedTime, string delimiter)
        {
            Id = id;
            Name = name;
            Alias = alias;
            Username = username;
            Password = password;
            Host = host;
            Port = port;
            Db = db;
            TimeoutSec = timeoutSec;
            ColorStop = colorEnd;
            ColorStart = colotStart;
            ServerType = serverType;
            LastConnectedTime = lastConnectedTime;
            Delimiter = delimiter;
        }

        public static async Task<bool> HadConnection()
        {
            var conn = await GetLastConnectedConnectionAsync();
            if (conn != null)
            {
                return true;
            }
            return false;
        }

        public static async Task UpdateDb(int id, int db)
        {
            DbCommand command = SqlliteHelper.CreateCommand();
            string commandText = $"UPDATE Connections SET Db = {db} WHERE Id = {id}";
            command.CommandText = commandText;
            await command.ExecuteScalarAsync();
        }

        public async Task<int> SaveAsync()
        {
            DbCommand command = SqlliteHelper.CreateCommand();
            await CreateTable(command);
            var name = Name == null || Name == "" ? Name + "''" : "'" + Name + "'";
            var alias = Alias == null || Alias == "" ? Alias + "''" : "'" + Alias + "'";
            var uname = Username == null || Username == "" ? Username + "''" : "'" + Username + "'";
            var passwd = Password == null || Password == "" ? Password + "''" : "'" + Password + "'";
            var cstart = "'#" + ColorStart.ToUInt32().ToString("X8") + "'";
            var cstop = "'#" + ColorStop.ToUInt32().ToString("X8") + "'";
            var host = Host == null || Host == "" ? Host + "''" : "'" + Host + "'";
            var serverType = ServerType == null || ServerType == "" ? ServerType + "''" : "'" + ServerType + "'";
            var lastConnectedTime = LastConnectedTime == null || LastConnectedTime == "" ? LastConnectedTime + "''" : "'" + LastConnectedTime + "'";
            var delimiter = Delimiter == null || Delimiter == "" ? Delimiter + "''" : "'" + Delimiter + "'";
            string commandText = "INSERT INTO Connections(Name, Alias, Username, Password, Host, Port, Db, TimeoutSec, ColorStop, ColorStart, ServerType, LastConnectedTime,Delimiter)" +
                $"VALUES ({name},{alias},{uname},{passwd},{host},{Port},{Db},{TimeoutSec},{cstart},{cstop},{serverType},{lastConnectedTime},{delimiter}) RETURNING Id;";
            command.CommandText = commandText;
            var res = await command.ExecuteScalarAsync();
            Id = Convert.ToInt32(res);
            return Id;
        }

        public static async Task<IEnumerable<Connection>> LoadAsync()
        {
            DbCommand command = SqlliteHelper.CreateCommand();
            await CreateTable(command);
            command.CommandText = "SELECT * FROM Connections";
            using var reader = await command.ExecuteReaderAsync();
            var connections = new List<Connection>();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string alias = reader.GetString(2);
                string username = reader.GetString(3);
                Color colorStart = Color.Parse(reader.GetString(4));
                Color colorEnd = Color.Parse(reader.GetString(5));
                string password = reader.GetString(6);
                string host = reader.GetString(7);
                int port = reader.GetInt32(8);
                int db = reader.GetInt32(9);
                int timeoutSec = reader.GetInt32(10);
                string serverType = reader.GetString(11);
                string lastConnectedTime = reader.GetString(12);
                string delimiter = reader.GetString(13);
                connections.Add(new Connection(id, name, alias, username, password, host, port, db,
                    timeoutSec, colorStart, colorEnd, serverType, lastConnectedTime, delimiter));
            }
            return connections;
        }

        private static async Task CreateTable(DbCommand command)
        {
            command.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name = 'Connections';";
            var res = await command.ExecuteScalarAsync();
            if (0 == Convert.ToInt32(res))
            {
                command.CommandText = @"
                                        CREATE TABLE Connections (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Name CHAR(50),
                                        Alias CHAR(50),
                                        Username CHAR(50),
                                        ColorStart char(8),
                                        ColorStop char(8),
                                        Password CHAR(50),
                                        Host CHAR(50),
                                        Port INTEGER,
                                        Db INTEGER,
                                        TimeoutSec INTEGER,
                                        ServerType char(20),
                                        LastConnectedTime char(20),
                                        Delimiter char(20)
                                      )";
                command.ExecuteNonQuery();
            }
        }

        public static async Task<int> Delete(int id)
        {
            DbCommand command = SqlliteHelper.CreateCommand();
            command.CommandText = $"DELETE FROM Connections WHERE Id = {id}";
            var res = await command.ExecuteNonQueryAsync();
            return res;
        }

        public static async Task UpdateLastConnectedTimeAsync(int id)
        {
            DbCommand command = SqlliteHelper.CreateCommand();
            string commandText = $"UPDATE Connections SET LastConnectedTime = {"'" + DateTime.Now.ToString() + "'"} WHERE Id = {id}";
            command.CommandText = commandText;
            await command.ExecuteScalarAsync();
        }

        public static async Task<Connection?> GetLastConnectedConnectionAsync()
        {
            DbCommand command = SqlliteHelper.CreateCommand();
            await CreateTable(command);
            string comandText = $"SELECT * FROM Connections where LastConnectedTime != '' order by LastConnectedTime desc limit 1;";
            command.CommandText = comandText;
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string alias = reader.GetString(2);
                string username = reader.GetString(3);
                Color colorStart = Color.Parse(reader.GetString(4));
                Color colorEnd = Color.Parse(reader.GetString(5));
                string password = reader.GetString(6);
                string host = reader.GetString(7);
                int port = reader.GetInt32(8);
                int db = reader.GetInt32(9);
                int timeoutSec = reader.GetInt32(10);
                string serverType = reader.GetString(11);
                string lastConnectedTime = reader.GetString(12);
                string delimiter = reader.GetString(13);
                Connection connection = new(id, name, alias, username, password, host, port, db,
                     timeoutSec, colorStart, colorEnd, serverType, lastConnectedTime, delimiter);
                reader.Close();
                return connection;
            }
            return null;
        }
    }

}
