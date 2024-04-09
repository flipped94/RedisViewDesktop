using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;

namespace RedisViewDesktopTest.Helpers
{
    [TestClass]
    public class CliOutputHelperTests
    {
        [TestMethod]
        [DataRow("Stack", "", "", "192.168.137.149", 6479, 0, 10)]
        public async Task TestFormat(string name, string username, string password, string host, int port, int db, int timesout)
        {
            Connection connection = new(name, username, password, host, port, db, timesout, ":");
            await RedisHelper.ConnectAsync(connection);
            var res = await RedisHelper.ExecuteAsync("XRANGE", ["race:france", "-", "+"]);
        }
    }
}