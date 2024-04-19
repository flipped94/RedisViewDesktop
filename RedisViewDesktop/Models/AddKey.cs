namespace RedisViewDesktop.Models
{
    public class AddKey(NewKey newKey, string key)
    {
        public NewKey NewKey { get; set; } = newKey;
        public string Key { get; set; } = key;
    }
}
