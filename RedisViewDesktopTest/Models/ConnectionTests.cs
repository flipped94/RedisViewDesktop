using RedisViewDesktop.Models;

namespace RedisViewDesktopTest.Models
{
    [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        [DataRow("Stack", "", "", "192.168.137.149", 6479, 0, 10)]
        public async Task TestSaveAsync(string name, string username, string password, string host, int port, int db, int timesout)
        {
            Connection connection = new(name, username, password, host, port, db, timesout,":");
            int id = await connection.SaveAsync();
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        [DataRow(1)]
        public async Task TestUpdateLastConnectedTime(int db)
        {
            await Connection.UpdateLastConnectedTimeAsync(db);
        }

        [TestMethod]
        public async Task TestGetLastConnectedConnectionAsync()
        {
            Connection? connection = await Connection.GetLastConnectedConnectionAsync();
            Assert.IsNotNull(connection);
        }
    }
}