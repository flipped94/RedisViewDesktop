using RedisViewDesktop.ViewModels;
using System;
using System.Collections.Generic;

namespace RedisViewDesktop.Helpers
{
    public class PaneHelper
    {
        private static readonly Dictionary<string, Type> SpliePaneDict = new Dictionary<string, Type>()
        {
            {"cli", typeof(PaneCliViewModel)},
            {"lua", typeof(PaneLuaViewModel)},
        };

        public static Type? GetPane(string pane)
        {
            if (SpliePaneDict.TryGetValue(pane, out var viewType))
            {
                return viewType;
            }
            return null;
        }
    }
}
