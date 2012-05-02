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
    /// 实现URL的管理
    /// </summary>
    [Serializable]
    public abstract class UrlManager:ISerializable,IDisposable
    {
        private IList<KeyValuePair<uint, Url>>[] lists; //根据URL的校验和的第一位数字,分组存储URL
        private Dictionary<uint, int>[] listsIndex;     //记录URL所在IList的索引位置,用于加速查找

        private Dictionary<int, int> listInitItems;     //记录每个分组的初始化数量,在ReadHistory方法被调用后设置
        private UrlManagePolicy policy;
        private static object locker = new object();
        private volatile bool dumpState;                //标记是否有线程正在进行Dump操作

        protected UrlManager()
        {
            this.dumpState = false;
            this.policy = this.GetManagePolicy();

            this.lists = new List<KeyValuePair<uint, Url>>[10];
            this.listsIndex = new Dictionary<uint, int>[10];
            this.listInitItems = new Dictionary<int, int>();

            for (int i = 0; i < 10; i++)
            {
                this.lists[i] = new List<KeyValuePair<uint, Url>>();
                this.listsIndex[i] = new Dictionary<uint, int>();
                this.listInitItems.Add(i, 0);
            }

        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected UrlManager(SerializationInfo info, StreamingContext context)
        {
            this.policy = info.GetValue("policy", typeof(UrlManagePolicy)) as UrlManagePolicy;
            UrlCollection urls = info.GetValue("urls", typeof(UrlCollection)) as UrlCollection;
            if (null != urls)
            {
                foreach (Url url in urls)
                {
                    this.AddOrRemoveItem(url, false);
                }
                urls.Clear();

                this.InitItemsCount();
            }
        }

        /// <summary>
        /// 初始化用于存储URL的哈希表
        /// </summary>
        internal void Init()
        {
            this.InitHistory();
            this.InitItemsCount();
        }

        private void InitItemsCount()
        {
            for (int i = 0; i < this.lists.Length; i++)
            {
                this.listInitItems[i] = this.lists[i].Count;
            }
        }

        /// <summary>
        /// 初始化历史数据
        /// </summary>
        private void InitHistory()
        {
            IList<Url> list = this.ReadHistory();
            if (null == list)
            {
                return;
            }

            foreach (Url u in list)
            {
                this.AddOrRemoveItem(u, false);
            }
        }

        /// <summary>
        /// 添加一个URL到当前的管理器,忽略起始URL
        /// </summary>
        /// <param name="url">Url</param>
        public void AddUrl(Url url)
        {
            if (null == url || url.GetType() == typeof(StartUrl))
            {
                return;
            }
            if (this.policy.UrlRepeatPolicy == UrlManagePolicy.AllowRepeatUrlPolicies.OnlyFinal && url.GetType() != typeof(FinalUrl))
            {
                return;
            }
            if (this.policy.UrlRepeatPolicy == UrlManagePolicy.AllowRepeatUrlPolicies.OnlyIndex && url.GetType() != typeof(IndexUrl))
            {
                return;
            }
            this.AddOrRemoveItem(url, false);
        }

        /// <summary>
        /// 从当前的管理器删除一个URL
        /// </summary>
        /// <param name="url"></param>
        public void RemoveUrl(Url url)
        {
            if (null == url)
            {
                return;
            }
            this.AddOrRemoveItem(url, true);
        }

        /// <summary>
        /// 检查给定的值是否存在于当前的管理器中
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>bool</returns>
        public bool IsExists(Url url)
        {
            return this.IsExistsKey(url.CheckSum);
        }

        /// <summary>
        /// 检查是否存在指定的CheckSum值
        /// </summary>
        /// <param name="checksum">CheckSum</param>
        /// <returns>bool</returns>
        public bool IsExistsKey(uint checksum)
        {
            Dictionary<uint, int> index = this.listsIndex[this.FindIndex(checksum)];
            lock (index)
            {
                return index.ContainsKey(checksum);
            }
        }

        /// <summary>
        /// 将CheckSum值添加到IList,或从中删除删除
        /// </summary>
        /// <param name="url">Url实例</param>
        /// <param name="isRemove">指示删除操作</param>
        private void AddOrRemoveItem(Url url, bool isRemove)
        {
            int i=this.FindIndex(url.CheckSum);
            IList<KeyValuePair<uint, Url>> list = this.lists[i];
            Dictionary<uint, int> index=this.listsIndex[i];

            if (isRemove)
            {
                list.RemoveAt(index[url.CheckSum]);
                index.Remove(url.CheckSum);
                return;
            }

            lock (locker)
            {
                if (!index.ContainsKey(url.CheckSum))
                {
                    list.Add(new KeyValuePair<uint, Url>(url.CheckSum, url));
                    index.Add(url.CheckSum, list.Count-1);
                }
            }
        }

        /// <summary>
        /// 寻找给定URL所在的目标表索引
        /// </summary>
        /// <returns>int</returns>
        private int FindIndex(Url url)
        {
            return this.FindIndex(url.CheckSum);
        }

        /// <summary>
        /// 根据checksum的第一位,寻找目标表,返回目标表的索引值
        /// </summary>
        /// <param name="checksum">checksum</param>
        /// <returns>int</returns>
        private int FindIndex(uint checksum)
        {
            int prefix = 0;
            int.TryParse(checksum.ToString().Substring(0, 1), out prefix);
            return prefix;
        }

        /// <summary>
        /// 在派生类中重写此方法,获取管理策略
        /// </summary>
        /// <returns>UrlManagePolicy</returns>
        public virtual UrlManagePolicy GetManagePolicy()
        {
            return UrlManagePolicy.Default;
        }

        /// <summary>
        /// 保存URL历史记录的增量部分
        /// </summary>
        public void Dump()
        {
            if (this.dumpState || null == this.lists)
            {
                return;
            }

            lock (locker)
            {
                this.dumpState = true;
                IList<Url> urls = this.GetMergedList();
                this.WriteHistory(urls);
                urls.Clear();

                //重置增量计数
                for (int i = 0; i < this.lists.Length; i++)
                {
                    this.listInitItems[i] = this.lists[i].Count;
                }

                this.dumpState = false;
            }
        }

        /// <summary>
        /// 将分组IList的增量部分合并到一个IList中
        /// </summary>
        /// <returns>IList</returns>
        private UrlCollection GetMergedList()
        {
            UrlCollection urls = new UrlCollection();
            for (int i = 0; i < this.lists.Length; i++)
            {
                for (int n = this.listInitItems[i]; n < this.lists[i].Count; n++)
                {
                    urls.Add(this.lists[i][n].Value);
                }
            }
            return urls;
        }

        /// <summary>
        /// 获取当前URL管理器中的项数
        /// </summary>
        public int Count
        {
            get
            {
                int n = 0;
                for (int i = 0; i < this.listsIndex.Length; i++)
                {
                    lock (listsIndex[i])
                    {
                        n += this.listsIndex[i].Count;
                    }
                }
                return n;
            }
        }

        /// <summary>
        /// 在派生类中重写此方法，用以初始化URL历史记录
        /// </summary>
        /// <returns>IList</returns>
        protected abstract IList<Url> ReadHistory();

        /// <summary>
        /// 在派生类中重写此方法，以保存历史URL
        /// <param name="urls">IList</param>
        /// </summary>
        protected abstract void WriteHistory(IList<Url> urls);

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            UrlCollection urls=this.GetMergedList();
            info.AddValue("policy",this.policy,typeof(UrlManagePolicy));
            info.AddValue("urls", urls, typeof(UrlCollection));
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (null != this.lists)
            {
                for (int i = 0; i < this.lists.Length; i++)
                {
                    this.lists[i].Clear();
                    this.lists[i] = null;
                }
            }

            if (null != this.listsIndex)
            {
                for (int i = 0; i < this.listsIndex.Length; i++)
                {
                    this.listsIndex[i].Clear();
                    this.listsIndex[i] = null;
                }
            }

            if (null != this.listInitItems)
            {
                this.listInitItems.Clear();
                this.listInitItems = null;
            }
        }

        #endregion

        ~UrlManager()
        {
            this.Dispose();
        }
    }
}