using RedisViewDesktop.ViewModels;
using System;
using System.Collections.Generic;

namespace RedisViewDesktop.Helpers
{
    public class KeyViewModleHelper
    {
        private static readonly Dictionary<string, Type> KeyViewModel = new()
        {

            { "HASH",typeof(HashViewModel) },
            { "STRING", typeof(StringViewModel) },
            { "LIST", typeof(ListViewModel) },
            { "SET", typeof(SetViewModel) },
            { "ZSET", typeof(ZSetViewModel) },
            { "REJSON-RL", typeof(JsonViewModel)},
            { "STREAM", typeof(StreamViewModel)},
            { "UNKNOWN", typeof(UnknownViewModel)}
        };

        public static Type GetViewType(string keyType)
        {
            if (KeyViewModel.TryGetValue(keyType, out var viewType))
            {
                return viewType;
            }
            return typeof(UnknownViewModel);
        }
    }
}
