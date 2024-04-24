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
            if (keyType is null)
            {
                return "#861b2d";
            }
            if (COLORS.TryGetValue(keyType, out var color))
            {
                return color;
            }
            else
            {
                return "#861b2d";
            }
        }

        public readonly static Dictionary<string, string> RedisTypeToAppTypeDict = new()
        {
            { "HASH", KeyTypeEnum.HASH.ToString() },
            { "STRING",KeyTypeEnum.STRING.ToString() },
            { "LIST" , KeyTypeEnum.LIST.ToString() },
            { "SET" , KeyTypeEnum.SET.ToString() },
            { "ZSET" , KeyTypeEnum.ZSET.ToString() },
            { "STREAM" , KeyTypeEnum.STREAM.ToString()},
            { "TOPK-TYPE" , KeyTypeEnum.TOPK.ToString()},
            { "CMSK-TYPE" , KeyTypeEnum.CMSK.ToString()},
            { "MBBLOOM--" , KeyTypeEnum.MBBLOOM.ToString()},
            { "MBBLOOMCF" , KeyTypeEnum.MBBLOOMCF.ToString()},
            { "TDIS-TYPE" , KeyTypeEnum.TDIS.ToString()},
            { "TSDB-TYPE" , KeyTypeEnum.TSDB.ToString()},
            { "REJSON-RL" , KeyTypeEnum.JSON.ToString()},
        };

        public static string RedisTypeToAppTypeString(string keyType)
        {
            if (RedisTypeToAppTypeDict.TryGetValue(keyType, out var type))
            {
                return type;
            }
            else
            {
                return "";
            }
        }

        public readonly static Dictionary<string, string> AppTypeToRedisTypeDict = new()
        {
            { KeyTypeEnum.HASH.ToString(),"HASH" },
            { KeyTypeEnum.STRING.ToString(),"STRING" },
            { KeyTypeEnum.LIST.ToString(), "LIST" },
            { KeyTypeEnum.SET.ToString(), "SET" },
            { KeyTypeEnum.ZSET.ToString(), "ZSET" },
            { KeyTypeEnum.STREAM.ToString(), "STREAM"},
            { KeyTypeEnum.TOPK.ToString(), "TOPK-TYPE"},
            { KeyTypeEnum.CMSK.ToString(), "CMSK-TYPE"},
            { KeyTypeEnum.MBBLOOM.ToString(), "MBBLOOM--"},
            { KeyTypeEnum.MBBLOOMCF.ToString(), "MBBLOOMCF"},
            { KeyTypeEnum.TDIS.ToString(), "TDIS-TYPE"},
            { KeyTypeEnum.TSDB.ToString(), "TSDB-TYPE"},
            { KeyTypeEnum.JSON.ToString(), "REJSON-RL"},
        };

        public static string? AppTypeToRedisTypeString(string keyType)
        {
            if (keyType is null)
            {
                return null;
            }
            if (AppTypeToRedisTypeDict.TryGetValue(keyType, out var type))
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
