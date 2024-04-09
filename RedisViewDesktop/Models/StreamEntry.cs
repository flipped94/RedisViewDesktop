namespace RedisViewDesktop.Models
{
    public class StreamEntry(string? entryID, string? value)
    {
        public string? EntryID { get; set; } = entryID;

        public string? Value { get; set; } = value;
    }
}