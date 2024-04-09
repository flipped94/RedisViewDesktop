using RedisViewDesktop.Enums;
using RedisViewDesktop.Helpers;
using System.Collections.Generic;
using System.Net;

namespace RedisViewDesktop.Models
{
    public class ScanKeyRequest(ScanKeyQequestBuilder builder)
    {
        public Dictionary<EndPoint, long> NodeCursor { get; set; } = builder.NodeCursor;
        public string Pattern { get; set; } = builder.Pattern;

        public int Count = builder.Count;
        public KeyTypeEnum KeyType { get; set; } = builder.KeyType;

        public object[] Args(EndPoint endPoint,long nodeCount)
        {
            List<object> args = [];          
            if (NodeCursor.TryGetValue(endPoint, out long cursor))
            {
                args.Add(cursor);
            }
            else
            {
                args.Add(0);
            }
            if (!string.IsNullOrEmpty(Pattern))
            {
                args.Add("MATCH");
                args.Add(Pattern);
            }
            if (Count > 0)
            {
                args.Add("COUNT");
                args.Add(Count/nodeCount>0?Count/nodeCount:Count/nodeCount+1);
            }
            else
            {
                args.Add("COUNT");
                args.Add(500/nodeCount);
            }
            var keyType = KeyTypeHelper.GetTypeString(KeyType);
            if (keyType != null)
            {
                args.Add("TYPE");
                args.Add(KeyType);
            }
            return [.. args];
        }

        public object[] Args(EndPoint endPoint)
        {
            List<object> args = [];
            if (NodeCursor.TryGetValue(endPoint, out long cursor))
            {
                args.Add(cursor);
            }
            else
            {
                args.Add(0);
            }
            if (!string.IsNullOrEmpty(Pattern))
            {
                args.Add("MATCH");
                args.Add(Pattern);
            }
            if (Count > 0)
            {
                args.Add("COUNT");
                args.Add(Count);
            }
            else
            {
                args.Add("COUNT");
                args.Add(300);
            }
            var keyType = KeyTypeHelper.GetTypeString(KeyType);
            if (keyType != null)
            {
                args.Add("TYPE");
                args.Add(KeyType);
            }
            return [.. args];
        }
    }

    public class ScanKeyQequestBuilder
    {
        public Dictionary<EndPoint, long> NodeCursor { get; set; } = [];
        public string Pattern { get; set; }
        public int Count { get; set; }
        public KeyTypeEnum KeyType { get; set; }

        public ScanKeyQequestBuilder SetPattern(string pattern)
        {
            Pattern = pattern;
            return this;
        }

        public ScanKeyQequestBuilder SetCount(int count)
        {
            Count = count;
            return this;
        }

        public ScanKeyQequestBuilder SetKeyType(KeyTypeEnum keyType)
        {
            KeyType = keyType;
            return this;
        }

        public ScanKeyQequestBuilder SetNodeCursor(Dictionary<EndPoint, long> nodeCursor)
        {
            NodeCursor = nodeCursor;
            return this;
        }

        public ScanKeyRequest Build()
        {
            return new ScanKeyRequest(this);
        }


    }

    public class ScanKeyResponse
    {
        public bool ShowMore { get; set; }

        public Dictionary<EndPoint, long> NodeCursor = [];

        public List<string> Keys { get; set; } = [];
    }

}
