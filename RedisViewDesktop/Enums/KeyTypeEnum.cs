namespace RedisViewDesktop.Enums
{
    public enum KeyTypeEnum
    {
        None = 0, 
        HASH,
        STRING,
        LIST,
        SET,
        ZSET,
        JSON,
        STREAM,
        TOPK,
        CMSK,
        MBBLOOM,
        MBBLOOMCF,
        TDIS,
        TSDB,
        UNKNOWN,
    }
}
