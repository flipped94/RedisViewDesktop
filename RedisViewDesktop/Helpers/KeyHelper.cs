using RedisViewDesktop.Enums;
using RedisViewDesktop.Models;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RedisViewDesktop.Helpers
{
    public class KeyHelper
    {
        private static readonly ConcurrentDictionary<string, KeyNode> KeyValuePairs = [];

        public static int NodeSort(KeyNode o1, KeyNode o2)
        {
            if (o1.IsKey && !o2.IsKey)
            {
                return 1;
            }
            if (!o1.IsKey && o2.IsKey)
            {
                return -1;
            }
            return o1.Name.CompareTo(o2.Name);
        }

        public static void DeleteTreeNode(string id)
        {
            while (KeyValuePairs.TryRemove(id, out KeyNode? node))
            {
                if (node is not null && node.ParentId is not null)
                {
                    if (KeyValuePairs.TryGetValue(node.ParentId, out KeyNode? keyNode))
                    {
                        if (keyNode is null || keyNode.KeyCount > 0)
                        {
                            break;
                        }
                    }
                    id = node.ParentId;
                }
            }
        }

        public static void Clear()
        {
            KeyValuePairs.Clear();
        }

        public static List<string> AllKeys()
        {
            return KeyValuePairs.Values.Where(x => x.IsKey).Select(x => x.Name).ToList();
        }

        public static List<KeyNode> LoadChildren(string parentId)
        {
            var children = KeyValuePairs.Values.Where(x => x.ParentId == parentId).ToList();
            children.Sort(NodeSort);
            return children;
        }

        public static List<KeyNode> Roots(bool isFirstLoad)
        {
            List<KeyNode> roots;
            if (isFirstLoad)
            {
                roots = KeyValuePairs.Values.Where(x => x.ParentId is null).Take(50).ToList();
            }
            else
            {
                roots = KeyValuePairs.Values.Where(x => x.ParentId is null).ToList();
            }

            roots.Sort(NodeSort);
            return roots;

        }

        public static void ConstructTreeNode(List<string> addedKeys, string delimiter = ":")
        {
            var start = DateTime.Now;
            ConcurrentDictionary<string, string> keyTypeDic = RedisHelper.KeyType(addedKeys);
            foreach (string name in addedKeys)
            {
                string[] nameSplitted = name.Split(delimiter);
                int lastIndex = nameSplitted.Length - 1;
                string? previous = null;
                for (int i = 0; i < nameSplitted.Length; i++)
                {
                    string? parentId = i == 0 ? null : previous;
                    string id = parentId == null ? nameSplitted[i] + delimiter : previous + nameSplitted[i] + delimiter;
                    KeyNode node = new()
                    {
                        ParentId = parentId,
                    };
                    previous = i == 0 ? nameSplitted[i] + delimiter : parentId + nameSplitted[i] + delimiter;
                    if (i != lastIndex)
                    {
                        // folder ndoe
                        node.Id = id;
                        node.Name = nameSplitted[i];
                        node.IsKey = false;
                        if (KeyValuePairs.ContainsKey(id))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // data node
                        node.Id = id + "k";
                        node.Name = name;
                        node.IsKey = true;
                        node.Type = KeyTypeHelper.RedisTypeToAppTypeString(keyTypeDic.GetValueOrDefault(name, "UNKNOWN").ToUpper());
                        node.Color = KeyTypeHelper.GetColor(node.Type);

                    }
                    KeyValuePairs.TryAdd(node.Id, node);
                }
            }
            Log.Error($"Build Tree Node Spent:{(DateTime.Now - start).TotalSeconds}");
        }

        public static List<KeyNode> BuildListNodes(List<string>? addKeys)
        {
            if (addKeys == null || addKeys.Count == 0)
            {
                return [];
            }
            ConcurrentDictionary<string, string> keyTypeDic = RedisHelper.KeyType([.. addKeys]);
            List<KeyNode> res = [];
            foreach (var item in keyTypeDic)
            {
                var type = KeyTypeHelper.RedisTypeToAppTypeString(item.Value);
                KeyNode node = new()
                {
                    Name = item.Key,
                    Type = type is null ? KeyTypeEnum.UNKNOWN.ToString() : type,
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

    }
}