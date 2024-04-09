using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using RedisViewDesktop.ViewModels;
using System.Diagnostics;

namespace RedisViewDesktopTest.Helpers
{
    [TestClass]
    public class RedisHelperTests
    {
        [TestMethod]
        [DataRow("Stack", "", "", "192.168.137.149", 6479, 0, 10)]
        public async Task TestKeyType(string name, string username, string password, string host, int port, int db, int timesout)
        {
            Connection connection = new(name, username, password, host, port, db, timesout, ":");
            await RedisHelper.ConnectAsync(connection);
            List<string> keys =
            [
                //"id:123:789",
                //"abc:456:789",
                //"abc:456",
                //"id:123:789:abc",
                //"abc:123",
                //"abc:123:456",
                //"id:123:789:def",
                //"id:123:456",
                "bikes:keywords",
            ];
            var keyype = RedisHelper.KeyType(keys);
            Debug.WriteLine(keyype.Count);
        }

        [TestMethod]
        [DataRow("Stack", "", "", "192.168.137.149", 6479, 0, 10)]
        public async Task TestHashDetail(string name, string username, string password, string host, int port, int db, int timesout)
        {
            Connection connection = new(name, username, password, host, port, db, timesout, ":");
            await RedisHelper.ConnectAsync(connection);
            var res = await RedisHelper.HashDetail("abc:hash");
            Debug.WriteLine($"Memory: {res["memory"]}");
            Debug.WriteLine($"Length: {res["len"]}");
            Debug.WriteLine($"TTL: {res["ttl"]}");
            IEnumerable<HashFieldViewModel> hashFieldViewModels = (IEnumerable<HashFieldViewModel>)res["content"];
            Debug.WriteLine("Fields: ");
            foreach (var item in hashFieldViewModels)
            {
                Debug.WriteLine($"\tField: {item.Field}\tValue: {item.Value}");
            }
        }

        [TestMethod]
        [DataRow("Stack", "", "", "192.168.137.149", 6479, 0, 10)]
        public async Task TestInfo(string name, string username, string password, string host, int port, int db, int timesout)
        {
            Connection connection = new(name, username, password, host, port, db, timesout, ":");
            await RedisHelper.ConnectAsync(connection);
            var infoMap = await RedisHelper.Info();
            foreach (var item in infoMap)
            {
                Debug.WriteLine($"{item.Key}:{item.Value}");
            }
        }

        [TestMethod]
        [DataRow("Stack", "", "", "192.168.137.149", 6479, 0, 10)]
        public async Task TestExecuteAsync(string name, string username, string password, string host, int port, int db, int timesout)
        {
            Connection connection = new(name, username, password, host, port, db, timesout, ":");
            await RedisHelper.ConnectAsync(connection);
            //var res =await RedisHelper.ExecuteAsync("info", null);
            _ = await RedisHelper.ExecuteAsync("XREVRANGE", ["race:france", "+", "-"]);
            //Debug.WriteLine(JsonHelper.Serialization(MultiBulk)res));
        }

        [TestMethod]
        public void TestRawString()
        {
            var str = @"ffff
                            dddd";
            Debug.WriteLine(str);
        }
    }
}
