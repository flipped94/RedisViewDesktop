using RedisViewDesktop.ViewModels;

namespace RedisViewDesktopTest.ViewModels
{
    [TestClass]
    public class KeysPageViewModelTests
    {
        [TestMethod]
        public async Task TestReady()
        {
            KeysPageViewModel keysPageViewModel = new();
            await keysPageViewModel.Ready();
        }
    }
}
