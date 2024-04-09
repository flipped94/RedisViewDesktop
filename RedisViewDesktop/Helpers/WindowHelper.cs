using Avalonia;
using Avalonia.Controls;

namespace RedisViewDesktop.Helpers
{
    public class WindowHelper
    {
        public static Window? FindParentWindow(Visual visual)
        {
            var parent = visual.Parent;
            while (parent != null)
            {
                if (parent is Window window)
                {
                    return window;
                }
                parent = parent.Parent;
            }
            return null;
        }
    }
}
