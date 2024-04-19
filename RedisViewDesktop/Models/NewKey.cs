using System.Collections.Generic;

namespace RedisViewDesktop.Models
{
    public class NewKey
    {
        public string Fill { get; set; }
        public string KeyType { get; set; }

        public NewKey(string fill, string keyType)
        {
            Fill = fill;
            KeyType = keyType;
        }

        public static IEnumerable<NewKey> GetNewKeys()
        {
            return new List<NewKey>()
            {
                new("#cdddf8","HASH"),
                new("#a5d4c3","LIST"),
                new("#d4baa7","SET"),
                new("#d9a0c6","ZSET"),
                new("#c7b0ea","STRING"),
                new("#b8c5db","JSON"),
                new("#c7cea8","STREAM")
            };
        }
    }
}
