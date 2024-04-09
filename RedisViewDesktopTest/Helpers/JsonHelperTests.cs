using RedisViewDesktop.Helpers;
using System.Diagnostics;

namespace RedisViewDesktopTest.Helpers
{
    [TestClass]
    public class JsonHelperTests
    {
        [TestMethod]
        public void FormatTest()
        {
            var json = JsonHelper.Format("{\"name\":\"John\", \"age\":30, \"city\":\"New York\"}");
            Debug.WriteLine(json);
        }
    }
}