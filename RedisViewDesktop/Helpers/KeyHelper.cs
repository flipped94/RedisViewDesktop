using DynamicData;
using RedisViewDesktop.Models;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedisViewDesktop.Helpers
{
    public class KeyHelper
    {     
        public static List<KeyNode> ToTree(List<string>? keys, string delimiter)
        {

            if (keys == null || keys.Count == 0)
            {
                return [];
            }
            List<KeyNode> keyNodes = BuildTreeNodes(keys, delimiter);

            return BuildTree(keyNodes);
        }

        public static List<KeyNode> BuildTree(List<KeyNode> nodes)
        {
            var start = DateTime.Now;
            List<KeyNode> roots = nodes.FindAll(node => null == node.ParentId);
            roots.Sort((o1, o2) =>
            {
                if (o1.Id.EndsWith('F') && o2.Id.EndsWith('F'))
                {
                    return o1.Id.CompareTo(o2.Id);
                }
                else if (o1.Id.EndsWith('F') && !o2.Id.EndsWith('F'))
                {
                    return -1;
                }
                else if (!o1.Id.EndsWith('F') && o2.Id.EndsWith('F'))
                {
                    return 1;
                }
                else
                {
                    return o1.Id.CompareTo(o2.Id);
                }
            });

            Dictionary<string, KeyNode> allMap = nodes.ToDictionary(o => o.Id);
            List<KeyNode> children = nodes.FindAll(o => o.ParentId != null);

            IEnumerable<IGrouping<string?, KeyNode>> group = children.GroupBy(o => o.ParentId);

            foreach (var item in group)
            {
                KeyNode parent;
                allMap.TryGetValue(item.Key!, out parent!);
                List<KeyNode> sub = [.. item];

                sub.Sort((o1, o2) =>
                {
                    if (o1.Id.EndsWith('F') && o2.Id.EndsWith('F'))
                    {
                        return o1.Id.CompareTo(o2.Id);
                    }
                    else if (o1.Id.EndsWith('F') && !o2.Id.EndsWith('F'))
                    {
                        return -1;
                    }
                    else if (!o1.Id.EndsWith('F') && o2.Id.EndsWith('F'))
                    {
                        return 1;
                    }
                    else
                    {
                        return o1.Id.CompareTo(o2.Id);
                    }
                });

                parent.Children.AddRange(sub);
            }
            Log.Error($"Build Tree Spent:{(DateTime.Now - start).TotalSeconds}");
            return roots;
        }

        public static List<KeyNode> BuildTreeNodes(List<string>? keys, string delimiter)
        {
            if (keys == null || keys.Count == 0)
            {
                return [];
            }
            Dictionary<string, KeyNode> keyValuePairs = [];
            ConcurrentDictionary<string, string> keyTypeDic = RedisHelper.KeyType(keys);
            var start = DateTime.Now;
            foreach (var key in keys)
            {
                string[] keySplited = key.Split(delimiter);
                int lastIndex = keySplited.Length - 1;

                for (int i = 0; i < keySplited.Length; i++)
                {
                    KeyNode node = new();

                    if (i == lastIndex)
                    {
                        node.Name = key;
                        node.Id = key + delimiter + "K";
                        node.IsKey = true;
                        node.Type = keyTypeDic.GetValueOrDefault(key, "UNKNOWN").ToUpper();
                        node.Color = KeyTypeHelper.GetColor(node.Type);
                    }
                    else
                    {
                        node.Name = keySplited[i];
                        node.Id = Merge(keySplited, i - 1, delimiter) + node.Name + delimiter + 'F';
                        node.IsKey = false;
                    }

                    if (i == 0)
                    {
                        node.ParentId = null;
                    }
                    else
                    {
                        node.ParentId = Merge(keySplited, i - 1, delimiter) + 'F';
                    }
                    keyValuePairs.TryAdd(node.Id, node);
                }
            }

            List<KeyNode> nodes = [.. keyValuePairs.Values];
            nodes.ForEach(node =>
            {
                if (node.IsKey)
                {
                    node.KeyCount = 0;
                }
                else
                {
                    node.KeyCount = keys.ToList().FindAll(s => s.StartsWith(node.Id[..^1])).Count;
                }
            });
            var duration = (DateTime.Now - start).TotalSeconds;
            Log.Error($"Build Tree Node Spent:{duration}");
            return nodes;
        }


        public static List<KeyNode> BuildListNodes(List<string>? keys)
        {
            if (keys == null || keys.Count == 0)
            {
                return [];
            }
            ConcurrentDictionary<string, string> keyTypeDic = RedisHelper.KeyType([.. keys]);
            List<KeyNode> res = [];
            foreach (var item in keyTypeDic)
            {
                KeyNode node = new()
                {
                    Name = item.Key,
                    Type = item.Value,
                    IsKey = true,
                    Color = KeyTypeHelper.GetColor(item.Value)
                };
                res.Add(node);
            }
            res.Sort((n1, n2) =>
            {
                return n1.Name.CompareTo(n2.Name);
            });
            return res;
        }

        private static string Merge(string[] splited, int index, string delimiter)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i <= index; i++)
            {
                stringBuilder.Append(splited[i]).Append(delimiter);
            }
            return stringBuilder.ToString();
        }
    }
}
