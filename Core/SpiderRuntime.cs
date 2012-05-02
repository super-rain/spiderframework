/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示爬虫运行时信息结构
    /// </summary>
    [Serializable]
    /// <summary>
    /// 表示爬虫运行时信息结构
    /// </summary>
    public sealed class SpiderRuntime
    {

        /// <summary>
        /// 即时采集线程数
        /// </summary>
        internal int crawlThreads;

        /// <summary>
        /// 即时处理线程数
        /// </summary>
        internal int processThreads;

        /// <summary>
        /// 即时URL队列长度
        /// </summary>
        internal int urlQueueLength;

        /// <summary>
        /// 即时内容队列长度
        /// </summary>
        internal int contentQueueLength;

        /// <summary>
        /// 即时已处理的URL数量
        /// </summary>
        internal int urlCount;

        /// <summary>
        /// 所处理的URL总数，包括索引URL
        /// </summary>
        internal int urlTotal;

        /// <summary>
        /// 已完成采集的字节数
        /// </summary>
        internal long bytesLoaded;

        /// <summary>
        /// 已发送的字节数
        /// </summary>
        internal long bytesSent;

        /// <summary>
        /// 已经运行的秒数
        /// </summary>
        internal double seconds;

        internal SpiderRuntime()
        {
            //
        }

        /// <summary>
        /// 即时采集线程数
        /// </summary>
        public int CrawlThreads
        {
            get
            {
                return this.crawlThreads;
            }
        }

        /// <summary>
        /// 即时处理线程数
        /// </summary>
        public int ProcessThreads
        {
            get
            {
                return this.processThreads;
            }
        }

        /// <summary>
        /// 即时URL队列长度
        /// </summary>
        public int UrlQueueLength
        {
            get
            {
                return this.urlQueueLength;
            }
        }

        /// <summary>
        /// 即时内容队列长度
        /// </summary>
        public int ContentQueueLength
        {
            get
            {
                return this.contentQueueLength;
            }
        }

        /// <summary>
        /// 即时已处理的URL数量
        /// </summary>
        public int UrlCount
        {
            get
            {
                return this.urlCount;
            }
        }

        /// <summary>
        /// 所处理的URL总数，包括索引URL
        /// </summary>
        public int UrlTotal
        {
            get
            {
                return this.urlTotal;
            }
        }

        /// <summary>
        /// 已完成采集的字节数
        /// </summary>
        public long BytesLoaded
        {
            get
            {
                return this.bytesLoaded;
            }
        }

        /// <summary>
        /// 已发送的字节数
        /// </summary>
        public long BytesSent
        {
            get
            {
                return this.bytesSent;
            }
        }

        /// <summary>
        /// 已经运行的秒数
        /// </summary>
        public double Seconds
        {
            get
            {
                return this.seconds;
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("采集线程:{0},处理线程:{1},URL队列:{2},内容队列:{3},已处理URL:{4},耗时:{5:0.000}秒,下载:{6},发送:{7},内存:{8}",
                this.CrawlThreads,
                this.ProcessThreads,
                this.UrlQueueLength,
                this.ContentQueueLength,
                this.UrlCount,
                this.Seconds,
                Utils.GetBytesFriendly(this.BytesLoaded),
                Utils.GetBytesFriendly(this.BytesSent),
                Utils.GetBytesFriendly(this.MemorySize)
            );
        }

        /// <summary>
        /// 获取当前的内存占用情况，以字节为单位
        /// </summary>
        public long MemorySize
        {
            get
            {
                return GC.GetTotalMemory(false);
            }
        }

    }
}
