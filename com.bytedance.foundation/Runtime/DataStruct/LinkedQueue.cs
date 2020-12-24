using System;
using System.Collections;
using System.Collections.Generic;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 用于支持从中间移除的队列结构（单类型版）
    /// 特点：
    ///     额外构建了一块内存用于按照 Key 值构建散列表索引到队列元素
    ///     快速索引 : O(1)
    ///     入队/查找元素 ： O(1)
    ///     移除元素 ： O(1)
    /// 用途：
    ///     适用于需要符合 FIFO 规则，同时还存在中部移除的情况。
    ///     使用 Key 与额外的空间用于构建队列元素散列表，但这里的 Key/Value 相等
    /// 实现：
    ///     内部使用双向链表 LinkedList 记录队列元素顺序。
    ///     每个新元素的写入会依据自身构建散列表，Value 为链表中的 Node
    /// 注意：
    ///     值不可重复。
    /// </summary>
    /// <typeparam name="TValue">队列元素类型</typeparam>
    public class LinkedQueue<TValue> : IDisposable, IEnumerable<TValue>
    {
        private LinkedQueue<TValue, TValue> internalQueue;

        public LinkedQueue(IEqualityComparer<TValue> comparer = null)
        {
            internalQueue = new LinkedQueue<TValue, TValue>(comparer);
        }
        public LinkedQueue(int capacity, IEqualityComparer<TValue> comparer = null)
        {
            internalQueue = new LinkedQueue<TValue, TValue>(capacity, comparer);
        }

        public int Count { get { return internalQueue.Count; } }

        public TValue Peek() { return internalQueue.Peek(); }

        public void Enqueue(TValue value)
        {
            internalQueue.Enqueue(value, value);
        }

        public TValue Dequeue()
        {
            return internalQueue.Dequeue();
        }

        public void Remove(TValue value)
        {
            internalQueue.RemoveByKey(value);
        }

        public bool Contains(TValue value)
        {
            return internalQueue.Contains(value);
        }

        /// <summary>
        /// 清理数据存储
        /// </summary>
        public void Clear()
        {
            internalQueue.Clear();
        }

        public void Dispose()
        {
            Clear();
            internalQueue = null;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return internalQueue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalQueue.GetEnumerator();
        }
    }


    /// <summary>
    /// 用于支持从中间移除的队列结构
    /// 特点：
    ///     额外构建了一块内存用于按照 Key 值构建散列表索引到队列元素
    ///     快速索引 : O(1)
    ///     入队/查找元素 ： O(1)
    ///     依据 Key 值移除元素 ： O(1)
    ///     依据 Value 值移除元素/出队 ： O(n)
    /// 用途：
    ///     适用于需要符合 FIFO 规则，同时还存在中部移除的情况。
    ///     使用 Key 与额外的空间用于构建队列元素散列表
    /// 实现：
    ///     内部使用双向链表 LinkedList 记录队列元素顺序。
    ///     每个新元素的写入会依据 Key 值构建散列表，Value 为链表中的 Node
    /// 注意：
    ///     Key 值不可重复。
    ///     Value 值原则上不允许重复，因为这会导致在依据 Value 移除的时候，散列表中的 Value 移除的对象不稳定
    /// </summary>
    /// <typeparam name="TKey">用于快速查找的 Key 值</typeparam>
    /// <typeparam name="TValue">队列元素类型</typeparam>
    public class LinkedQueue<TKey, TValue> : IDisposable, IEnumerable<TValue>
    {
        private LinkedList<TValue> list = new LinkedList<TValue>();
        private Dictionary<TKey, LinkedListNode<TValue>> hashMap;

        public LinkedQueue(IEqualityComparer<TKey> comparer = null)
        {
            hashMap = new Dictionary<TKey, LinkedListNode<TValue>>(comparer);
        }
        public LinkedQueue(int capacity, IEqualityComparer<TKey> comparer = null)
        {
            hashMap = new Dictionary<TKey, LinkedListNode<TValue>>(capacity, comparer);
        }

        public TValue Peek()
        {
            var first = list?.First;
            return first == null ? default : first.Value;
        }

        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// 注意：需要确保 key 值存在，set 只用于修改
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                LinkedListNode<TValue> node = null;
                if (hashMap.TryGetValue(key, out node))
                    return node.Value;
                else
                    MyLogger.LogError_("Try to get value of key(not found):" + key.ToString());
                return default;
            }
            set
            {
                LinkedListNode<TValue> node = null;
                if (hashMap.TryGetValue(key, out node))
                    node.Value = value;
                else
                    MyLogger.LogError_("Try to modify value of key(not found):" + key.ToString());
            }
        }

        /// <summary>
        /// 入队
        /// </summary>
        public void Enqueue(TKey key, TValue value)
        {
            if (hashMap.ContainsKey(key))
                MyLogger.LogError_("LinkedQueue key can't be repeated.");
            else
                hashMap.Add(key, list.AddLast(value));
        }

        /// <summary>
        /// 出队
        /// </summary>
        public TValue Dequeue()
        {
            if (list.Count < 1)
            {
                MyLogger.LogError_("LinkedQueue has nothing to dequeue.");
                return default;
            }

            var first = list.First;
            list.RemoveFirst();

            bool isFound = false;
            TKey keyCache = default;
            foreach (var pair in hashMap)
            {
                if (pair.Value == first)
                {
                    isFound = true;
                    keyCache = pair.Key;
                    break;
                }
            }
            Assert.AssertTrue(isFound, "LinkedQueue has unexpected error, LinkedListNode not found.");
            if (isFound)
                hashMap.Remove(keyCache);

            return first.Value;
        }

        /// <summary>
        /// 移除指定元素[高速版]
        /// </summary>
        public void RemoveByKey(TKey key)
        {
            LinkedListNode<TValue> node = null;
            if (hashMap.TryGetValue(key, out node))
            {
                hashMap.Remove(key);
                list.Remove(node);
            }
            else
            {
                MyLogger.LogError_("Remove Failed. key:" + key);
            }
        }

        /// <summary>
        /// 移除指定元素[开销较大，推荐使用 key 执行 Remove 的重载]
        /// </summary>
        /// <param name="value"></param>
        public void Remove(TValue value)
        {
            bool isFound = false;
            TKey keyCache = default;
            LinkedListNode<TValue> nodeCache = default;
            foreach (var pair in hashMap)
            {
                if (pair.Value.Value.Equals(value))
                {
                    isFound = true;
                    keyCache = pair.Key;
                    nodeCache = pair.Value;
                    break;
                }
            }
            Assert.AssertTrue(isFound, "LinkedQueue has unexpected error, LinkedListNode not found.");
            if (isFound)
            {
                list.Remove(nodeCache);
                hashMap.Remove(keyCache);
            }
        }

        /// <summary>
        /// 检测是否包含指定修改
        /// </summary>
        public bool Contains(TKey key)
        {
            return hashMap.ContainsKey(key);
        }

        /// <summary>
        /// 尝试获取某个 key 值对应的 Record 对象
        /// </summary>
        public bool TryGetValue(TKey key, out TValue record)
        {
            record = default;
            bool ret = false;
            LinkedListNode<TValue> recNode = null;
            if (hashMap.TryGetValue(key, out recNode))
            {
                ret = true;
                record = recNode.Value;
            }
            return ret;
        }

        /// <summary>
        /// 修改现有数据，维持排序关系
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        public void ModifyValue(TKey key, TValue newValue)
        {
            LinkedListNode<TValue> curNode = null;
            if (!hashMap.TryGetValue(key, out curNode))
            {
                MyLogger.LogError_("Try to modify a null key-value pair.");
            }
            else
            {
                var newNode = list.AddAfter(curNode, newValue);
                hashMap[key] = newNode;
                list.Remove(curNode);
            }
        }

        /// <summary>
        /// 清理数据存储
        /// </summary>
        public void Clear()
        {
            hashMap.Clear();
            list.Clear();
        }

        public void Dispose()
        {
            Clear();
            hashMap = null;
            list = null;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
