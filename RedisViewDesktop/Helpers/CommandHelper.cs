using AvaloniaEdit.Utils;
using System.Collections.Generic;

namespace RedisViewDesktop.Helpers
{
    public class CommandHelper
    {
        // admin commands
        private static readonly Dictionary<string, string[]> AdminCommandsDict = new()
        {
            {
                  "ACL", new string[] {
                      "ACL CAT [categoryname]",
                      "ACL DELUSER username [username ...]",
                      "ACL DRYRUN username command [arg [arg ...]]",
                      "ACL GENPASS [bits]",
                      "ACL GETUSER username",
                      "ACL LIST", "ACL LOAD",
                      "ACL LOG [count|RESET]",
                      "ACL SAVE",
                      "ACL SETUSER username [rule [rule ...]]",
                      "ACL USERS",
                      "ACL WHOAMI"
                  }
            },
            {
                "BGREWRITEAOF", new string[] { "BGREWRITEAOF" }
            },
            {
                "BGSAVE", new string [] { "BGSAVE" }
            },
            {
                "CLIENT", new string[] { "CLIENT LIST [TYPE normal|master|replica|pubsub]", "CLIENT GETNAME", "CLIENT REPLY ON|OFF|SKIP", "CLIENT ID" }
            },
            {
                "CLUSTER", new string[] {
                    "CLUSTER COUNT-FAILURE-REPORTS node-id",
                    "CLUSTER COUNTKEYSINSLOT slot",
                    "CLUSTER GETKEYSINSLOT slot count",
                    "CLUSTER INFO",
                    "CLUSTER KEYSLOT key",
                    "CLUSTER NODES",
                    "CLUSTER SLAVES node-id",
                    "CLUSTER REPLICAS node-id",
                    "CLUSTER SLOTS"
                }
            },
            {
                "CONFIG", new string[] { "CONFIG GET parameter", "CONFIG SET parameter value", "CONFIG RESETSTAT" }
            },
            {
                "DEBUG", new string[] { "DEBUG OBJECT key", "DEBUG SEGFAULT" }
            },
            {
                "FAILOVER", new string[] { "FAILOVER [TO host port [FORCE]] [ABORT] [TIMEOUT milliseconds]" }
            },
            {
                "LATENCY", new string[] {
                    "LATENCY DOCTOR",
                    "LATENCY GRAPH event",
                    "LATENCY HISTOGRAM [command [command ...]]",
                    "LATENCY HISTORY event",
                    "LATENCY LATEST",
                    "LATENCY RESET [event [event ...]]"
                }
            },
            {
                "MODULE", new string[] { "MODULE LIST", "MODULE LOAD path [arg [arg ...]]", "MODULE UNLOAD name" }
            },
            {
                "MONITOR", new string[] { "MONITOR" }
            },
            {
                "PFDEBUG", new string[] { "PFDEBUG" }
            },
            {
                "PFSELFTEST", new string[] { "PFSELFTEST" }
            },
            {
                "PSYNC", new string[] { "PSYNC replicationid offset" }
            },
            {
                "REPLCONF", new string[] { "REPLCONF" }
            },
            {
                "REPLICAOF", new string[] { "REPLICAOF host port" }
            },
            {
                "SAVE", new string[] { "SAVE" }
            },
            {
                "SHUTDOWN", new string[] { "SHUTDOWN [NOSAVE|SAVE] [NOW] [FORCE] [ABORT]" }
            },
            {
                "SLAVEOF", new string[] { "SLAVEOF host port" }
            },
            {
                "SLOWLOG", new string[] { "SLOWLOG subcommand [argument]" }
            },
            {
                "SYNC", new string[] { "SYNC" }
            }
        };

        // read commands
        private static readonly Dictionary<string, string[]> ReadCommandsDict = new()
        {
            {
                "AUTH", new string[] { "AUTH password" }
            },
            {
                "BITCOUNT", new string[] { "BITCOUNT key [start] [end]" }
            },
            {
                "BITOP", new string[] { "BITOP operation destkey key [key ...]" }
            },
            {
                "BITPOS", new string[] { "BITPOS key bit [start] [end]" }
            },
            {
                "COMMAND", new string[] { "COMMAND COUNT", "COMMAND GETKEYS", "COMMAND INFO command-name [command-name ...]" }
            },
            {
                "DBSIZE", new string[] { "DBSIZE" }
            },
            {
                "DISCARD", new string[] { "DISCARD" }
            },
            {
                "DUMP", new string[] { "DUMP key" }
            },
            {
                "ECHO", new string[] { "ECHO message" }
            },
            {
                "EXEC", new string[] { "EXEC" }
            },
            {
                "EXISTS", new string[] { "EXISTS key" }
            },
            {
                "GET", new string[] { "GET key" }
            },
            {
                "GETBIT", new string[] { "GETBIT key offset" }
            },
            {
                "GETRANGE", new string[] { "GETRANGE key start end" }
            },
            {
                "HEXISTS", new string[] { "HEXISTS key field" }
            },
            {
                "HGET", new string[] { "HGET key field" }
            },
            {
                "HGETALL", new string[] { "HGETALL key" }
            },
            {
                "HKEYS", new string[] { "HKEYS key" }
            },
            {
                "HLEN", new string[] { "HLEN key" }
            },
            {
                "HMGET", new string[] { "HMGET key field [field ...]" }
            },
            {
                "HSCAN", new string[] { "HSCAN key cursor [MATCH pattern] [COUNT count]" }
            },
            {
                "HVALS", new string[] { "HVALS key" }
            },
            {
                "INFO", new string[] { "INFO [section]" }
            },
            {
                "KEYS", new string[] { "KEYS pattern" }
            },
            {
                "LASTSAVE", new string[] { "LASTSAVE" }
            },
            {
                "LINDEX", new string[] { "LINDEX key index" }
            },
            {
                "LLEN", new string[] { "LLEN key" }
            },
            {
                "LRANGE", new string[] { "LRANGE key start stop" }
            },
            {
                "MGET", new string[] { "MGET key [key ...]" }
            },
            {
                "MULTI", new string[] { "MULTI" }
            },
            {
                "OBJECT", new string[] { "OBJECT ENCODING key", "OBJECT FREQ key", "OBJECT IDLETIME key", "OBJECT REFCOUNT key" }
            },
            {
                "PING", new string[] { "PING [message]" }
            },
            {
                "PUBSUB", new string[] { "PUBSUB CHANNELS [pattern]", "PUBSUB NUMSUB [channel-1 ...]", "PUBSUB NUMPAT" }
            },
            {
                "PSUBSCRIBE", new string[] { "PSUBSCRIBE pattern [pattern ...]" }
            },
            {
                "PTTL", new string[] { "PTTL key" }
            },
            {
                "PUNSUBSCRIBE", new string[] { "PUNSUBSCRIBE [pattern ...]" }
            },
            {
                "RANDOMKEY", new string[] { "RANDOMKEY" }
            },
            {
                "ROLE", new string[] { "ROLE" }
            },
            {
                "SCAN", new string[] { "SCAN cursor [MATCH pattern] [COUNT count]" }
            },
            {
                "SCARD", new string[] { "SCARD key" }
            },
            {
                "SDIFF", new string[] { "SDIFF key [key ...]" }
            },
            {
                "SELECT", new string[] { "SELECT index" }
            },
            {
                "SINTER", new string[] { "SINTER key [key ...]" }
            },
            {
                "SISMEMBER", new string[] { "SISMEMBER key member" }
            },
            {
                "SMEMBERS", new string[] { "SMEMBERS key" }
            },
            {
                "SRANDMEMBER", new string[] { "SRANDMEMBER key [count]" }
            },
            {
                "SSCAN", new string[] { "SSCAN key cursor [MATCH pattern] [COUNT count]" }
            },
            {
                "STRLEN", new string[] { "STRLEN key" }
            },
            {
                "SUBSCRIBE", new string[] { "SUBSCRIBE channel [channel ...]" }
            },
            {
                "SUNION", new string[] { "SUNION key [key ...]" }
            },
            {
                "TIME", new string[] { "TIME" }
            },
            {
                "TTL", new string[] { "TTL key" }
            },
            {
                "TYPE", new string[] { "TYPE key" }
            },
            {
                "UNSUBSCRIBE", new string[] { "UNSUBSCRIBE [channel ...]" }
            },
            {
                "UNWATCH", new string[] { "UNWATCH" }
            },
            {
                "WATCH", new string[] { "WATCH key [key ...]" }
            },
            {
                "ZCARD", new string[] { "ZCARD key" }
            },
            {
                "ZCOUNT", new string[] { "ZCOUNT key min max" }
            },
            {
                "ZLEXCOUNT", new string[] { "ZLEXCOUNT key min max" }
            },
            {
                "ZRANGE", new string[] { "ZRANGE key start stop [WITHSCORES]" }
            },
            {
                "ZRANGEBYLEX", new string[] { "ZRANGEBYLEX key min max [LIMIT offset count]" }
            },
            {
                "ZRANGEBYSCORE", new string[] { "ZRANGEBYSCORE key min max [WITHSCORES] [LIMIT offset count]" }
            },
            {
                "ZRANK", new string[] { "ZRANK key member" }
            },
            {
                "ZREVRANGE", new string[] { "ZREVRANGE key start stop [WITHSCORES]" }
            },
            {
                "ZREVRANGEBYLEX", new string[] { "ZREVRANGEBYLEX key max min [LIMIT offset count]" }
            },
            {
                "ZREVRANGEBYSCORE", new string[] { "ZREVRANGEBYSCORE key max min [WITHSCORES] [LIMIT offset count]" }
            },
            {
                "ZREVRANK", new string[] { "ZREVRANK key member" }
            },
            {
                "ZSCAN", new string[] { "ZSCAN key cursor [MATCH pattern] [COUNT count]" }
            },
            {
                "ZSCORE", new string[] { "ZSCORE key member" }
            },
            {
                "GEOHASH", new string[] { "GEOHASH key member [member ...]" }
            },
            {
                "GEOPOS", new string[] { "GEOPOS key member [member ...]" }
            },
            {
                "GEODIST", new string[] { "GEODIST key member1 member2 [unit]" }
            },
            {
                "HSTRLEN", new string[] { "HSTRLEN key field" }
            },
            {
                "MEMORY", new string[] { "MEMORY DOCTOR", "MEMORY HELP", "MEMORY MALLOC-STATS", "MEMORY STATS", "MEMORY USAGE key [SAMPLES count]" }
            },
            {
                "XINFO", new string[] { "XINFO [CONSUMERS key groupname] [GROUPS key] [STREAM key] [HELP]" }
            },
            {
                "XRANGE", new string[] { "XRANGE key start end [COUNT count]" }
            },
            {
                "XREVRANGE", new string[] { "XREVRANGE key end start [COUNT count]" }
            },
            {
                "XLEN", new string[] { "XLEN key" }
            },
            {
                "XREAD", new string[] { "XREAD [COUNT count] [BLOCK milliseconds] STREAMS key [key ...] ID [ID ...]" }
            },
            {
                "XREADGROUP", new string[] { "XREADGROUP GROUP group consumer [COUNT count] [BLOCK milliseconds] [NOACK] STREAMS key [key ...] ID [ID ...]" }
            },
            {
                "XPENDING", new string[] { "XPENDING key group [start end count] [consumer]" }
            },
        };

        // write commands
        private static readonly Dictionary<string, string[]> WriteCommandsDict = new()
        {
            {
                "APPEND", new string[] { "APPEND key value" }
            },
            {
                "BLMOVE", new string[] { "BLMOVE source destination LEFT|RIGHT LEFT|RIGHT timeout" }
            },
            {
                "BLPOP", new string[] { "BLPOP key [key ...] timeout" }
            },
            {
                "BRPOP", new string[] { "BRPOP key [key ...] timeout" }
            },
            {
                "BRPOPLPUSH", new string[] { "BRPOPLPUSH source destination timeout" }
            },
            {
                "BZPOPMAX", new string[] { "BZPOPMAX key [key ...] timeout" }
            },
            {
                "BZPOPMIN", new string[] { "BZPOPMIN key [key ...] timeout" }
            },
            {
                "COPY", new string[] { "COPY source destination [DB destination-db] [REPLACE]" }
            },
            {
                "DECR", new string[] { "DECR key" }
            },
            {
                "DECRBY", new string[] { "DECRBY key decrement" }
            },
            {
                "DEL", new string[] { "DEL key [key ...]" }
            },
            {
                "EVAL", new string[] { "EVAL script numkeys key [key ...] arg [arg ...]" }
            },
            {
                "EVALSHA", new string[] { "EVALSHA sha1 numkeys key [key ...] arg [arg ...]" }
            },
            {
                "EXPIRE", new string[] { "EXPIRE key seconds" }
            },
            {
                "EXPIREAT", new string[] { "EXPIREAT key timestamp" }
            },
            {
                "FLUSHALL", new string[] { "FLUSHALL" }
            },
            {
                "FLUSHDB", new string[] { "FLUSHDB" }
            },
            {
                "GEOADD", new string[] { "GEOADD key [NX|XX] [CH] longitude latitude member [longitude latitude member ...]" }
            },
            {
                "GETDEL", new string[] { "GETDEL key" }
            },
            {
                "GETSET", new string[] { "GETSET key value" }
            },
            {
                "HDEL", new string[] { "HDEL key field [field ...]" }
            },
            {
                "HINCRBY", new string[] { "HINCRBY key field increment" }
            },
            {
                "HINCRBYFLOAT", new string[] { "HINCRBYFLOAT key field increment" }
            },
            {
                "HMSET", new string[] { "HMSET key field value [field value ...]" }
            },
            {
                "HSET", new string[] { "HSET key field value" }
            },
            {
                "HSETNX", new string[] { "HSETNX key field value" }
            },
            {
                "INCR", new string[] { "INCR key" }
            },
            {
                "INCRBY", new string[] { "INCRBY key increment" }
            },
            {
                "INCRBYFLOAT", new string[] { "INCRBYFLOAT key increment" }
            },
            {
                "LINSERT", new string[] { "LINSERT key BEFORE|AFTER pivot value" }
            },
            {
                "LMOVE", new string[] { "LMOVE source destination LEFT|RIGHT LEFT|RIGHT" }
            },
            {
                "LPOP", new string[] { "LPOP key" }
            },
            {
                "LPUSH", new string[] { "LPUSH key value [value ...]" }
            },
            {
                "LPUSHX", new string[] { "LPUSHX key value" }
            },
            {
                "LREM", new string[] { "LREM key count value" }
            },
            {
                "LSET", new string[] { "LSET key index value" }
            },
            {
                "LTRIM", new string[] { "LTRIM key start stop" }
            },
            {
                "MIGRATE", new string[] { "MIGRATE host port key destination-db timeout" }
            },
            {
                "MOVE", new string[] { "MOVE key db" }
            },
            {
                "MSET", new string[] { "MSET key value [key value ...]" }
            },
            {
                "MSETNX", new string[] { "MSETNX key value [key value ...]" }
            },
            {
                "PERSIST", new string[] { "PERSIST key" }
            },
            {
                "PEXPIRE", new string[] { "PEXPIRE key milliseconds" }
            },
            {
                "PEXPIREAT", new string[] { "PEXPIREAT key milliseconds-timestamp" }
            },
            {
                "PSETEX", new string[] { "PSETEX key milliseconds value" }
            },
            {
                "PUBLISH", new string[] { "PUBLISH channel message" }
            },
            {
                "RENAME", new string[] { "RENAME key newkey" }
            },
            {
                "RENAMENX", new string[] { "RENAMENX key newkey" }
            },
            {
                "RESTORE", new string[] { "RESTORE key ttl serialized-value" }
            },
            {
                "RPOP", new string[] { "RPOP key" }
            },
            {
                "RPOPLPUSH", new string[] { "RPOPLPUSH source destination" }
            },
            {
                "RPUSH", new string[] { "RPUSH key value [value ...]" }
            },
            {
                "RPUSHX", new string[] { "RPUSHX key value" }
            },
            {
                "SADD", new string[] { "SADD key member [member ...]" }
            },
            {
                "SCRIPT", new string[] { "SCRIPT EXISTS script [script ...]", "SCRIPT FLUSH", "SCRIPT KILL", "SCRIPT LOAD script" }
            },
            {
                "SDIFFSTORE", new string[] { "SDIFFSTORE destination key [key ...]" }
            },
            {
                "SET", new string[] { "SET key value" }
            },
            {
                "SETBIT", new string[] { "SETBIT key offset value" }
            },
            {
                "SETEX", new string[] { "SETEX key seconds value" }
            },
            {
                "SETNX", new string[] { "SETNX key value" }
            },
            {
                "SETRANGE", new string[] { "SETRANGE key offset value" }
            },
            {
                "SINTERSTORE", new string[] { "SINTERSTORE destination key [key ...]" }
            },
            {
                "SMOVE", new string[] { "SMOVE source destination member" }
            },
            {
                "SORT", new string[] { "SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]" }
            },
            {
                "SPOP", new string[] { "SPOP key" }
            },
            {
                "SREM", new string[] { "SREM key member [member ...]" }
            },
            {
                "SUNIONSTORE", new string[] { "SUNIONSTORE destination key [key ...]" }
            },
            {
                "SWAPDB", new string[] { "SWAPDB index1 index2" }
            },
            {
                "UNLINK", new string[] { "UNLINK key [key ...]" }
            },
            {
                "XADD", new string[] { "XADD key ID field string [field string ...]" }
            },
            {
                "XDEL", new string[] { "XDEL key ID [ID ...]" }
            },
            {
                "XGROUP", new string[] {
                    "XGROUP CREATE key groupname id|$ [MKSTREAM]",
                    "XGROUP CREATECONSUMER key groupname consumername",
                    "XGROUP DELCONSUMER key groupname consumername",
                    "XGROUP DESTROY key groupname", "XGROUP SETID key groupname id|$"}
            },
            {
                "XTRIM", new string[] { "XTRIM key MAXLEN [~] count" }
            },
            {
                "ZADD", new string[] { "ZADD key score member [score] [member]" }
            },
            {
                "ZDIFFSTORE", new string[] { "ZDIFFSTORE destination numkeys key [key ...]" }
            },
            {
                "ZINCRBY", new string[] { "ZINCRBY key increment member" }
            },
            {
                "ZINTERSTORE", new string[] { "ZINTERSTORE destination numkeys key [key ...] [WEIGHTS weight [weight ...]] [AGGREGATE SUM|MIN|MAX]" }
            },
            {
                "ZPOPMAX", new string[] { "ZPOPMAX key [count]" }
            },
            {
                "ZPOPMIN", new string[] { "ZPOPMIN key [count]" }
            },
            {
                "ZRANGESTORE", new string[] { "ZRANGESTORE dst src min max [BYSCORE|BYLEX] [REV] [LIMIT offset count]" }
            },
            {
                "ZREM", new string[] { "ZREM key member [member ...]" }
            },
            {
                "ZREMRANGEBYLEX", new string[] { "ZREMRANGEBYLEX key min max" }
            },
            {
                "ZREMRANGEBYRANK", new string[] { "ZREMRANGEBYRANK key start stop" }
            },
            {
                "ZREMRANGEBYSCORE", new string[] { "ZREMRANGEBYSCORE key min max" }
            },
            {
                "ZUNIONSTORE", new string[] { "ZUNIONSTORE destination numkeys key [key ...] [WEIGHTS weight [weight ...]] [AGGREGATE SUM|MIN|MAX]" }
            },
        };

        // all commands
        private static readonly Dictionary<string, string[]> AllCommandsDict = [];

        public static bool IsReadCommand(string command)
        {
            return !WriteCommandsDict.ContainsKey(command);
        }

        public static List<string> GetTips(string command)
        {
            InitAllCommandds();
            List<string> results = [];

            foreach (var kv in AllCommandsDict)
            {
                if (kv.Key.StartsWith(command, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    results.AddRange(kv.Value);
                }
            }
            return results;
        }

        private static void InitAllCommandds()
        {
            if (AllCommandsDict.Count == 0)
            {
                AllCommandsDict.AddRange(AdminCommandsDict);
                AllCommandsDict.AddRange(ReadCommandsDict);
                AllCommandsDict.AddRange(WriteCommandsDict);
            }
        }
    }

}
