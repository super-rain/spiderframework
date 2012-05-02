/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 队列的抽象基类
    /// </summary>
    /// <typeparam name="T">泛型队列元素</typeparam>
    [Serializable]
    public abstract class QueueBase<T> : ISerializable,IDisposable
    {
        private Queue queue;

        /// <summary>
        /// 构造函数
        /// </summary>
        protected QueueBase()
        {
            this.queue = Queue.Synchronized(new Queue());
        }

        protected QueueBase(SerializationInfo info, StreamingContext context)
        {
            this.queue = Queue.Synchronized(info.GetValue("queue", typeof(Queue)) as Queue);
        }

        protected Queue BaseQueue
        {
            get
            {
                return this.queue;
            }
        }


        /// <summary>
        /// 排队一个元素
        /// </summary>
        /// <param name="item">待排队的元素</param>
        public void Add(T item)
        {
            if (null==item)
            {
                return;
            }
            this.queue.Enqueue(item);
        }

        /// <summary>
        /// 将URL集合依次添加到队列的末尾,Add方法的批量实现
        /// </summary>
        /// <param name="urls">URL集合</param>
        public void AddRange(IList<T> items)
        {
            foreach (T item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// 返回队列的第一个元素,并从队列中移除
        /// </summary>
        /// <returns>Url</returns>
        public T Next()
        {
            lock (this.queue.SyncRoot)
            {
                if (this.queue.Count == 0)
                {
                    return default(T);
                }

                return (T)this.queue.Dequeue();
            }
        }

        /// <summary>
        /// 获取当期队列的长度
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.queue.SyncRoot)
                {
                    return this.queue.Count;
                }
            }
        }

        #region ISerializable 成员

        /// <summary>
        /// 提供序列化支持(ISerializable)
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("queue", this.queue, typeof(Queue));
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (null != this.queue)
            {
                this.queue.Clear();
            }
        }

        #endregion

        ~QueueBase()
        {
            this.Dispose();
        }
    }
}
