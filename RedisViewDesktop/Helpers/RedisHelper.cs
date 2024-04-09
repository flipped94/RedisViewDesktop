using NRedisStack;
using NRedisStack.RedisStackCommands;
using RedisViewDesktop.Models;
using RedisViewDesktop.ViewModels;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StreamEntry = RedisViewDesktop.Models.StreamEntry;

namespace RedisViewDesktop.Helpers
{
    public class RedisHelper
    {
        private RedisHelper() { }
        private static IDatabase? database;
        private static ConnectionMultiplexer? multiplexer;
        private static bool IsClusterMode = false;

        public static async Task TestConnectAsync(Connection connection)
        {
            await ConnectAsyncpublic(connection);
        }

        public static async Task ConnectAsync(Connection connection)
        {
            try
            {
                multiplexer = await ConnectAsyncpublic(connection);
                IServer server = multiplexer.GetServer(connection.Host, connection.Port);
                connection.ServerType = server.ServerType.ToString();
                IsClusterMode = ServerType.Cluster.ToString().Equals(connection.ServerType);
                if ("Cluster".Equals(connection.ServerType))
                {
                    connection.Db = 0;
                }
                database = multiplexer.GetDatabase();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private static async Task<ConnectionMultiplexer> ConnectAsyncpublic(Connection connection)
        {
            ConfigurationOptions options = new()
            {
                EndPoints = { { connection.Host, connection.Port } },
                Password = connection.Password,
                ConnectTimeout = connection.TimeoutSec * 1000,
                DefaultDatabase = connection.Db,
                AllowAdmin = true,
            };
            ConnectionMultiplexer multiplexer = await ConnectionMultiplexer.ConnectAsync(options);
            return multiplexer;
        }

        public static async Task<ExecuteResult> ExecuteAsync(string command, object[] args)
        {
            if (null == args || args.Length == 0)
            {
                try
                {
                    var res = await database!.ExecuteAsync(command);
                    return new ExecuteResult(true, CliOutputHelper.Format(res));
                }
                catch (Exception e)
                {
                    return new ExecuteResult(false, RedisResult.Create(new RedisValue(e.Message)));
                }
            }
            else
            {
                try
                {
                    var res = await database!.ExecuteAsync(command, args);
                    return new ExecuteResult(true, CliOutputHelper.Format(res));
                }
                catch (Exception e)
                {
                    return new ExecuteResult(false, RedisResult.Create(new RedisValue(e.Message)));
                }
            }

        }

        public static async Task<Dictionary<string, object>> Info()
        {
            var redisResult = await database.ExecuteAsync("INFO");
            var splits = ((string?)redisResult).Split("\r\n");
            var infos = splits.Where(x => !x.StartsWith('#') && !string.IsNullOrEmpty(x)).Select(x => x);
            Dictionary<string, object> infoDic = [];
            foreach (var item in infos)
            {
                var info = item.Split(":");
                infoDic.TryAdd(info[0], info[1].Trim());
            }
            return infoDic;
        }

        public static void SwitchDb(int db)
        {
            var newDb = multiplexer!.GetDatabase(db);
            database = newDb;
        }

        public static async Task<long> DbSize()
        {
            var replicas = multiplexer.GetServers().Where(x => !x.IsReplica).ToArray();
            List<long> keys = [];
            foreach (var replica in replicas)
            {
                var res = await replica.ExecuteAsync("DBSIZE", null);
                keys.Add(((long)res));
            }
            return keys.Sum();
        }

        public static async Task<ScanKeyResponse> Scan(ScanKeyRequest request, bool isInit = true)
        {
            if (IsClusterMode)
            {
                var nodes = multiplexer.GetServers().Where(x => !x.IsReplica).ToArray();
                if (null != nodes && nodes.Length > 0)
                {
                    Dictionary<EndPoint, RedisResult> resDict = [];
                    foreach (var replica in nodes)
                    {
                        if (!isInit)
                        {
                            request.NodeCursor.TryGetValue(replica.EndPoint, out long cursor);
                            if (0 == cursor)
                            {
                                continue;
                            }
                        }
                        object[] args = request.Args(replica.EndPoint, nodes.Length);
                        var res = await replica.ExecuteAsync("SCAN", args);
                        resDict.TryAdd(replica.EndPoint, res);
                    }

                    ScanKeyResponse response = new();
                    Dictionary<EndPoint, long> cursorDict = [];
                    foreach (var keyValuePair in resDict)
                    {
                        var result = keyValuePair.Value;
                        if (result == null)
                        {
                            response.NodeCursor.TryAdd(keyValuePair.Key, 0);
                        }
                        else
                        {
                            var res = (RedisResult[]?)result;
                            if (res != null && res[0] != null && !res[0].IsNull)
                            {
                                response.NodeCursor.TryAdd(keyValuePair.Key, (long)res[0]);
                            }
                            if (res != null && res[1] != null && !res[1].IsNull)
                            {
                                string[]? keys = (string[]?)res[1];
                                if (null != keys && keys.Length > 0)
                                {
                                    response.Keys.AddRange(keys);
                                }
                            }
                        }
                    }
                    response.ShowMore = response.NodeCursor.Where(x => x.Value > 0).Any();
                    return response;
                }
            }
            else
            {
                if (!isInit)
                {
                    request.NodeCursor.TryGetValue(database.IdentifyEndpoint(), out long cursor);
                    if (cursor == 0)
                    {
                        return new ScanKeyResponse();
                    }
                }
                RedisResult result = await database!.ExecuteAsync("SCAN", request.Args(database.IdentifyEndpoint()!));

                if (!result.IsNull)
                {
                    var res = (RedisResult[]?)result;
                    if (res == null)
                    {
                        return new ScanKeyResponse();
                    }
                    ScanKeyResponse response = new();
                    if (!res[0].IsNull)
                    {
                        Dictionary<EndPoint, long> cursor = [];
                        cursor.TryAdd(database.IdentifyEndpoint(), ((long)res[0]));
                        response.NodeCursor = cursor;
                        response.ShowMore = ((long)res[0]) > 0;
                    }
                    if (!res[1].IsNull)
                    {
                        string[]? keys = (string[]?)res[1];
                        if (keys != null)
                        {
                            response.Keys = [.. keys];
                        }
                    }
                    return response;
                }
            }
            return new ScanKeyResponse();
        }

        public static async Task<bool> DeleteKey(string key)
        {
            return await database.KeyDeleteAsync(key);
        }

        public static ConcurrentDictionary<string, string> KeyType(List<string> keys)
        {
            ConcurrentDictionary<string, string> keyType = new();
            Task[] tasks = new Task[keys.Count];
            IBatch batch = database!.CreateBatch();
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                tasks[i] = batch.ExecuteAsync("type", [key])
                     .ContinueWith((Task<RedisResult> type) =>
                     {
                         keyType.TryAdd(key, type.Result.ToString()!.ToUpper());
                     });
            }
            batch.Execute();
            Task.WaitAll(tasks);
            return keyType;
        }

        public static async Task<Dictionary<string, string>> StringDetail(string key)
        {
            Dictionary<string, string> detail = [];
            var memeory = await database!.ExecuteAsync("MEMORY", ["USAGE", key]);
            var len = await database!.StringLengthAsync(key);
            TimeSpan? ttl = await database!.KeyTimeToLiveAsync(key);
            var conent = await database.StringGetAsync(key);
            detail.Add("memory", memeory + "");
            detail.Add("len", len + "");
            if (null != ttl)
            {
                detail.Add("ttl", ttl.Value.TotalSeconds + "");
            }
            else
            {
                detail.Add("ttl", "No Limit");
            }
            detail.Add("content", conent);
            return detail;
        }

        public static async Task<bool> StringSet(string key, string value)
        {
            return await database.StringSetAsync(key, value);
        }

        public static async Task<Dictionary<string, object>> ZSetDetail(string key)
        {
            Dictionary<string, object> detail = [];
            var memeory = await database!.ExecuteAsync("MEMORY", ["USAGE", key]);
            var len = await database!.SortedSetLengthAsync(key);
            TimeSpan? ttl = await database!.KeyTimeToLiveAsync(key);
            var conent = await database.SortedSetRangeByRankWithScoresAsync(key, 0, -1, Order.Ascending);
            detail.Add("memory", memeory + "");
            detail.Add("len", len + "");
            if (null != ttl)
            {
                detail.Add("ttl", ttl.Value.TotalSeconds + "");
            }
            else
            {
                detail.Add("ttl", "No Limit");
            }
            IEnumerable<ZSetMemberViewModel> members = conent.Select(x => new ZSetMemberViewModel(key, x.Element, x.Score));
            detail.Add("content", members);
            return detail;
        }

        public static async Task<bool> ZSetDeleteMember(string key, string member)
        {
            return await database.SortedSetRemoveAsync(key, member);
        }

        public static async Task<bool> ZSetUpdateScore(string key, string member, double score)
        {
            return await database.SortedSetUpdateAsync(key, member, score);
        }

        public static async Task<long> ZSetAddMembersAsync(string key, Dictionary<string, double> entrys)
        {
            var s = entrys.Select(kv => new SortedSetEntry(kv.Key, kv.Value)).ToArray();
            return await database.SortedSetAddAsync(key, s);
        }

        public static async Task<Dictionary<string, object>> HashDetail(string key)
        {
            Dictionary<string, object> detail = [];
            var memeory = await database!.ExecuteAsync("MEMORY", ["USAGE", key]);
            var len = await database!.HashLengthAsync(key);
            TimeSpan? ttl = await database!.KeyTimeToLiveAsync(key);
            HashEntry[] entries = await database.HashGetAllAsync(key);
            detail.Add("memory", memeory + "");
            detail.Add("len", len + "");

            if (null != ttl)
            {
                detail.Add("ttl", ttl.Value.TotalSeconds + "");
            }
            else
            {
                detail.Add("ttl", "No Limit");
            }
            IEnumerable<HashFieldViewModel> fileds = entries.Select(x => new HashFieldViewModel(key, x.Name, x.Value));
            detail.Add("content", fileds);

            return detail;
        }

        public static async Task<bool> HashDeleteField(string key, string field)
        {
            return await database.HashDeleteAsync(key, field);
        }

        public static async Task HasAddFieldsAsync(string key, Dictionary<string, string> entrys)
        {
            var s = entrys.Select(kv => new HashEntry(kv.Key, kv.Value)).ToArray();
            await database.HashSetAsync(key, s);
        }

        public static async Task<Dictionary<string, object>> ListDetail(string key)
        {
            Dictionary<string, object> detail = [];
            var memeory = await database!.ExecuteAsync("MEMORY", ["USAGE", key]);
            var len = await database!.ListLengthAsync(key);
            TimeSpan? ttl = await database!.KeyTimeToLiveAsync(key);
            RedisValue[] values = await database.ListRangeAsync(key);
            detail.Add("memory", memeory + "");
            detail.Add("len", len + "");

            if (null != ttl)
            {
                detail.Add("ttl", ttl.Value.TotalSeconds + "");
            }
            else
            {
                detail.Add("ttl", "No Limit");
            }
            IEnumerable<string> elements = values.Select(x => x.ToString());
            detail.Add("content", elements);

            return detail;
        }

        public static async Task<long> ListRemoveLAsync(string key, string data)
        {
            return await database.ListRemoveAsync(key, data, 1);
        }

        public static async Task ListEditAsync(string key, string pivot, string value)
        {
            _ = database.ListInsertAfter(key, pivot, value);
            await database.ListRemoveAsync(key, pivot, 1);
        }

        public static async Task ListRpushAcync(string key, string[] data)
        {
            var args = new object[data.Length + 1];
            args[0] = key;
            for (int i = 1; i < args.Length; i++)
            {
                args[i] = data[i - 1];
            }

            _ = await database.ExecuteAsync("RPUSH", args);
        }

        public static async Task<Dictionary<string, object>> SetDetail(string key)
        {
            Dictionary<string, object> detail = [];
            var memeory = await database!.ExecuteAsync("MEMORY", ["USAGE", key]);
            var len = await database!.SetLengthAsync(key);
            TimeSpan? ttl = await database!.KeyTimeToLiveAsync(key);
            var values = await database.SetMembersAsync(key);
            detail.Add("memory", memeory + "");
            detail.Add("len", len + "");

            if (null != ttl)
            {
                detail.Add("ttl", ttl.Value.TotalSeconds + "");
            }
            else
            {
                detail.Add("ttl", "No Limit");
            }
            detail.Add("content", values.Select(x => x.ToString()).ToHashSet());

            return detail;
        }

        public static async Task<bool> SetRemoveAsync(string key, string selectedMember)
        {
            return await database.SetRemoveAsync(key, selectedMember);
        }

        public static async Task SetAddMembersAsync(string key, HashSet<string> members)
        {
            var s = members.ToArray();
            object[] args = new object[members.Count + 1];
            args[0] = key;
            for (int i = 1; i < args.Length; i++)
            {
                args[i] = s[i - 1];
            }
            await database.ExecuteAsync("SADD", args);
        }

        public static async Task SetEditAsync(string key, string oldValue, string newValue)
        {
            await database.SetRemoveAsync(key, oldValue);
            await database.SetAddAsync(key, newValue);
        }

        public static async Task<Dictionary<string, object>> JsonDetail(string key)
        {
            Dictionary<string, object> detail = [];
            var bytes = await database!.JSON().DebugMemoryAsync(key);
            var hash = await database.JSON().ObjKeysAsync(key);
            var keys = hash.FirstOrDefault([]);
            var res = database.JSON().Get(key);

            TimeSpan? ttl = await database!.KeyTimeToLiveAsync(key);
            detail.Add("memory", bytes + "");
            detail.Add("len", keys.Count() + "");

            if (null != ttl)
            {
                detail.Add("ttl", ttl.Value.TotalSeconds + "");
            }
            else
            {
                detail.Add("ttl", "No Limit");
            }
            detail.Add("content", (string?)res);
            return detail;
        }

        public static async Task<Dictionary<string, object>> StreamDetail(string key)
        {
            Dictionary<string, object> detail = [];
            var memeory = await database!.ExecuteAsync("MEMORY", ["USAGE", key]);
            var len = await database!.StreamLengthAsync(key);
            TimeSpan? ttl = await database!.KeyTimeToLiveAsync(key);
            var messages = await database.StreamRangeAsync(key);
            detail.Add("memory", memeory + "");
            detail.Add("len", len + "");

            if (null != ttl)
            {
                detail.Add("ttl", ttl.Value.TotalSeconds + "");
            }
            else
            {
                detail.Add("ttl", "No Limit");
            }
            var res = messages.OrderBy(x => x.Id);
            IEnumerable<StreamEntry> elements = res.Select(x => new StreamEntry(x.Id, JsonHelper.Serialization(x.Values)));
            detail.Add("content", elements);

            return detail;
        }
    }
}
