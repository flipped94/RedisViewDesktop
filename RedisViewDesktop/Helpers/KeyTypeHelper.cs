using RedisViewDesktop.Enums;
using System.Collections.Generic;

namespace RedisViewDesktop.Helpers
{
    public class KeyTypeHelper
    {

        private readonly static Dictionary<string, string> COLORS = new()
        {

            { "HASH","#364cff" },
            { "STRING","#6a1dc3" },
            { "LIST", "#008556" },
            { "SET", "#a00a6b" },
            { "ZSET", "#9c5c2b" },
            { "STREAM", "#6a741b"},
            { "TOPK-TYPE", "#aa4e4e"},
            { "CMSK-TYPE", "#aa4e4e"},
            { "MBBLOOM--", "#aa4e4e"},
            { "MBBLOOMCF", "#aa4e4e"},
            { "TDIS-TYPE", "#aa4e4e"},
            { "TSDB-TYPE", "#6e6e6e"},
            { "REJSON-RL", "#3f4b5f"},
            { "UNKNOWN", "#861b2d"},
        };

        public static string GetColor(string keyType)
        {
            if (COLORS.TryGetValue(keyType, out var color))
            {
                return color;
            }
            else
            {
                return "#861b2d";
            }
        }

        public readonly static Dictionary<KeyTypeEnum, string> KEYTYPE = new()
        {
            { KeyTypeEnum.HASH,"HASH" },
            { KeyTypeEnum.STRING,"STRING" },
            { KeyTypeEnum.LIST, "LIST" },
            { KeyTypeEnum.SET, "SET" },
            { KeyTypeEnum.ZSET, "ZSET" },
            { KeyTypeEnum.STREAM, "STREAM"},
            { KeyTypeEnum.TOPK, "TOPK-TYPE"},
            { KeyTypeEnum.CMSK, "CMSK-TYPE"},
            { KeyTypeEnum.MBBLOOM, "MBBLOOM--"},
            { KeyTypeEnum.MBBLOOMCF, "MBBLOOMCF"},
            { KeyTypeEnum.TDIS, "TDIS-TYPE"},
            { KeyTypeEnum.TSDB, "TSDB-TYPE"},
            { KeyTypeEnum.JSON, "REJSON-RL"},
        };

        public static string? GetTypeString(KeyTypeEnum keyTypeEnum)
        {
            if (KEYTYPE.TryGetValue(keyTypeEnum, out var type))
            {
                return type;
            }
            else
            {
                return null;
            }
        }
    }
}
