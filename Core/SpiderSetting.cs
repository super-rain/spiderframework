/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示爬虫的配置信息
    /// </summary>
    [Serializable]
    public class SpiderSetting : ISerializable
    {
        #region CONST
        /// <summary>
        /// 爬行深度所允许的最大值
        /// </summary>
        public const short MaxDepthNumber = 10;

        /// <summary>
        /// 请求超时的最大值
        /// </summary>
        public const int MaxRequestTimeout = 60000;

        /// <summary>
        /// 读写超时的最大值
        /// </summary>
        public const int MaxIOTimeout = 120000;

        /// <summary>
        /// 爬行线程数所允许的最大值
        /// </summary>
        public const short MaxCrawlThreads = 32;

        /// <summary>
        /// 内容处理线程数所允许的最大值
        /// </summary>
        public const short MaxProcessThreads = 32;

        /// <summary>
        /// 允许的读取缓冲区的最大值
        /// </summary>
        public const int MaxReadBufferSize = 1048576;   //1MB

        /// <summary>
        /// 允许使用内存流直接存储的最大字节数
        /// </summary>
        public const int MaxMemLimitSize = 1048576;     //1MB

        /// <summary>
        /// 默认的User-Agent字符串
        /// </summary>
        public const string DefaultUserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)";

        #endregion

        /// <summary>
        /// 采集速度模式
        /// </summary>
        [Serializable]
        public enum SpeedModes
        {
            /// <summary>
            /// 极速模式,普通模式的10倍,并且不会进行失败重试
            /// </summary>
            VeryFast = 10,

            /// <summary>
            /// 快速模式,普通模式的5倍,并且不会进行失败重试
            /// </summary>
            Fast = 20,

            /// <summary>
            /// 普通,每秒10次请求
            /// </summary>
            Normal = 100,

            /// <summary>
            /// 慢速模式,普通模式的0.5倍
            /// </summary>
            Slow = 200,

            /// <summary>
            /// 极慢模式,普通模式的0.2倍
            /// </summary>
            VerySlow = 500,

            /// <summary>
            /// 龟速模式,普通模式的0.02倍
            /// </summary>
            Tortoise = 5000
        }

        private string name;                        //爬虫名称
        private StartUrl startUrl;                  //起始URL
        private short maxDepth;                     //最大爬行深度
        private bool allowRedirect;                 //是否允许转向
        private int requestTimeout;                 //请求超时的毫秒数
        private int iOTimeout;                      //读写超时的毫秒数
        private int readBufferSize;                 //读取数据的缓冲区大小
        private short crawlThreads;                 //采集的线程数
        private short processThreads;               //内容处理的线程数
        private IWebProxy proxy;                    //网络代理
        private string userAgent;                   //User-Agent
        private string referer;                     //引用URL
        private CookieCollection cookies;           //Cookie集合
        private Encoding requestEncoding;           //请求编码

        private UrlExtractor urlExtractor;          //URL抽取器
        private ContentHandlerCollection contentHandlers;  //内容处理程序集合
        private ILogger logger;                     //日志程序

        private int memLimitSize;                   //使用内存流存储的二进制内容的限制字节数,超过此长度的二进制内容将使用文件流直接存储,默认100K
        private string depositePath;                //附件存储路径

        private SpeedModes speedMode;                //采集速度模式


        /// <summary>
        /// 构造函数
        /// </summary>
        public SpiderSetting()
        {
            this.maxDepth = 5;
            this.requestTimeout = 0;
            this.iOTimeout = 0;
            this.readBufferSize = 1024;
            this.crawlThreads = 1;
            this.processThreads = 1;
            this.userAgent = DefaultUserAgent;
            this.referer = "";
            this.requestEncoding = Encoding.Default;

            this.memLimitSize = 102400;
            this.depositePath = "";

            this.speedMode = SpeedModes.Normal;

            this.contentHandlers = new ContentHandlerCollection();
        }

        protected SpiderSetting(SerializationInfo info, StreamingContext context)
        {
            this.name = info.GetString("name");
            this.startUrl = info.GetValue("startUrl", typeof(StartUrl)) as StartUrl;
            this.maxDepth = info.GetInt16("maxDepth");
            this.allowRedirect = info.GetBoolean("allowRedirect");
            this.requestTimeout = info.GetInt32("requestTimeout");
            this.iOTimeout = info.GetInt32("iOTimeout");
            this.readBufferSize = info.GetInt32("readBufferSize");
            this.crawlThreads = info.GetInt16("crawlThreads");
            this.processThreads = info.GetInt16("processThreads");
            this.proxy = info.GetValue("proxy", typeof(IWebProxy)) as IWebProxy;
            this.userAgent = info.GetString("userAgent");
            this.referer = info.GetString("referer");
            this.cookies = info.GetValue("cookies", typeof(CookieCollection)) as CookieCollection;
            this.requestEncoding = info.GetValue("requestEncoding", typeof(Encoding)) as Encoding;
            this.urlExtractor = info.GetValue("urlExtractor", typeof(UrlExtractor)) as UrlExtractor;
            this.contentHandlers = info.GetValue("contentHandlers", typeof(ContentHandlerCollection)) as ContentHandlerCollection;
            this.logger = info.GetValue("logger", typeof(ILogger)) as ILogger;
            this.memLimitSize = info.GetInt32("memLimitSize");
            this.depositePath = info.GetString("depositePath");
            this.speedMode = (SpeedModes)info.GetValue("speedMode",typeof(SpeedModes));
        }

        /// <summary>
        /// 获取或设置名称
        /// </summary>
        public string Name
        {
            get
            {
                if (null == this.name || "" == this.name)
                {
                    if (null != this.startUrl)
                    {
                        return this.startUrl.Uri.Host;
                    }
                    return "UnnamedSpider";
                }
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// 获取或设置起始URL
        /// </summary>
        public StartUrl StartUrl
        {
            get
            {
                return this.startUrl;
            }
            set
            {
                this.startUrl = value;
            }
        }

        /// <summary>
        /// 获取或设置最大爬行深度,相对与根路径,允许的最大值为常量:MaxDepthNumber
        /// </summary>
        public short MaxDepth
        {
            get
            {
                return this.maxDepth;
            }
            set
            {
                this.maxDepth = value;
                if (this.maxDepth < 1)
                {
                    this.maxDepth = 1;
                }
                if (this.maxDepth > MaxDepthNumber)
                {
                    this.maxDepth = MaxDepthNumber;
                }
            }
        }

        /// <summary>
        /// 获取或设置是否允许URL跳转的抓取
        /// </summary>
        public bool AllowRedirect
        {
            get
            {
                return this.allowRedirect;
            }
            set
            {
                this.allowRedirect = value;
            }
        }

        /// <summary>
        /// 获取或设置请求超时的毫秒数,允许的最大值为常量:MaxRequestTimeout
        /// </summary>
        public int RequestTimeout
        {
            get
            {
                return this.requestTimeout;
            }
            set
            {
                this.requestTimeout = value;
                if (this.requestTimeout < 0)
                {
                    this.requestTimeout = 0;
                }
                if (this.requestTimeout > MaxRequestTimeout)
                {
                    this.requestTimeout = MaxRequestTimeout;
                }
            }
        }

        /// <summary>
        /// 获取或设置数据读写的超时毫秒数,允许的最大值为常量:MaxIOTimeout
        /// </summary>
        public int IOTimeout
        {
            get
            {
                return this.iOTimeout;
            }
            set
            {
                this.iOTimeout = value;
                if (this.iOTimeout < 0)
                {
                    this.iOTimeout = 0;
                }
                if (this.iOTimeout > MaxIOTimeout)
                {
                    this.iOTimeout = MaxIOTimeout;
                }
            }
        }

        /// <summary>
        /// 获取或设置读缓冲区字节数,最小值1024
        /// </summary>
        public int ReadBufferSize
        {
            get
            {
                return this.readBufferSize;
            }
            set
            {
                this.readBufferSize = value;
                if (this.readBufferSize < 1024)
                {
                    this.readBufferSize = 1024;
                }
                if (this.readBufferSize > MaxReadBufferSize)
                {
                    this.readBufferSize = MaxReadBufferSize;
                }
            }
        }

        /// <summary>
        /// 获取或设置抓取的线程数,允许的最大值为常量:MaxCrawlThreads
        /// </summary>
        public short CrawlThreads
        {
            get
            {
                return this.crawlThreads;
            }
            set
            {
                this.crawlThreads = value;
                if (this.crawlThreads < 1)
                {
                    this.crawlThreads = 1;
                }
                if (this.crawlThreads > MaxCrawlThreads)
                {
                    this.crawlThreads = MaxCrawlThreads;
                }
            }
        }

        /// <summary>
        /// 获取或设置内容处理的线程数,允许的最大值为常量:MaxProcessThreads
        /// </summary>
        public short ProcessThreads
        {
            get
            {
                return this.processThreads;
            }
            set
            {
                this.processThreads = value;
                if (this.processThreads < 1)
                {
                    this.processThreads = 1;
                }
                if (this.processThreads > MaxProcessThreads)
                {
                    this.processThreads = MaxProcessThreads;
                }
            }
        }

        /// <summary>
        /// 获取或设置代理实例,未定义则返回NULL
        /// </summary>
        public IWebProxy Proxy
        {
            get
            {
                return this.proxy;
            }
            set
            {
                this.proxy = value;
            }
        }

        /// <summary>
        /// 获取或设置请求头的User-Agent设置
        /// </summary>
        public string UserAgent
        {
            get
            {
                return this.userAgent;
            }
            set
            {
                this.userAgent = value;
            }
        }

        /// <summary>
        /// 获取或设置URLREFERER
        /// </summary>
        public string Referer
        {
            get
            {
                return this.referer;
            }
            set
            {
                this.referer = value;
            }
        }

        /// <summary>
        /// 获取或设置Cookie集合
        /// </summary>
        public CookieCollection Cookies
        {
            get
            {
                return this.cookies;
            }
            set
            {
                this.cookies = value;
            }
        }

        /// <summary>
        /// 获取或设置请求的编码方式
        /// </summary>
        public Encoding RequestEncoding
        {
            get
            {
                return this.requestEncoding;
            }
            set
            {
                this.requestEncoding = value;
            }
        }

        /// <summary>
        /// 获取或设置URL抽取器
        /// </summary>
        public UrlExtractor UrlExtractor
        {
            get
            {
                return this.urlExtractor;
            }
            set
            {
                this.urlExtractor = value;
            }
        }


        /// <summary>
        /// 获取或设置内容处理程序集合
        /// </summary>
        public ICollection<IContentHandler> ContentHandlers
        {
            get
            {
                return this.contentHandlers;
            }
        }

        /// <summary>
        /// 获取或设置日志处理程序,该处理程序必须实现ILogger接口
        /// </summary>
        public ILogger Logger
        {
            get
            {
                return this.logger;
            }
            set
            {
                this.logger = value;
            }
        }

        /// <summary>
        /// 获取或设置使用内存流存储的二进制内容的限制字节数,超过此长度的二进制内容将使用文件流直接存储,允许的最大值为常量:MaxMemLimitSize
        /// </summary>
        public int MemLimitSize
        {
            get
            {
                return this.memLimitSize;
            }
            set
            {
                this.memLimitSize = value;
                if (this.memLimitSize > MaxMemLimitSize)
                {
                    this.memLimitSize = MaxMemLimitSize;
                }
            }
        }

        /// <summary>
        /// 获取或设置内容存储路径,如果路径不存在将引发DirectoryNotFoundException
        /// </summary>
        public string DepositePath
        {
            get
            {
                return this.depositePath;
            }
            set
            {
                if (!System.IO.Directory.Exists(value))
                {
                    throw new System.IO.DirectoryNotFoundException("Directory does not exists:" + value);
                }
                this.depositePath = value;
                if (!this.depositePath.EndsWith("\\"))
                {
                    this.depositePath += "\\";
                }
            }
        }

        public SpeedModes SpeedMode
        {
            get
            {
                return this.speedMode;
            }
            set
            {
                this.speedMode = value;
            }
        }

        /// <summary>
        /// 用当前的设置初始化一个WebRequest实例
        /// </summary>
        /// <param name="request">WebRequest</param>
        internal void PrepareRequest(HttpWebRequest request)
        {
            if (null == request)
            {
                return;
            }

            if (this.allowRedirect)
            {
                request.AllowAutoRedirect = this.allowRedirect;
                request.MaximumAutomaticRedirections = 2;
            }

            if (null != this.cookies)
            {
                if (null == request.CookieContainer)
                {
                    request.CookieContainer = new CookieContainer();
                }
                request.CookieContainer.Add(this.cookies);
            }

            if (this.iOTimeout > 0)
            {
                request.ReadWriteTimeout = this.iOTimeout;
            }

            if (null != this.proxy)
            {
                request.Proxy = this.proxy;
            }

            if (this.referer != "")
            {
                request.Referer = this.referer;
            }

            if (this.requestTimeout > 0)
            {
                request.Timeout = this.requestTimeout;
            }

            if (this.userAgent != "")
            {
                request.UserAgent = this.userAgent;
            }
        }

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", this.name);
            info.AddValue("startUrl", this.startUrl, typeof(StartUrl));
            info.AddValue("maxDepth", this.maxDepth);
            info.AddValue("allowRedirect", this.allowRedirect);
            info.AddValue("requestTimeout", this.requestTimeout);
            info.AddValue("iOTimeout", this.iOTimeout);
            info.AddValue("readBufferSize", this.readBufferSize);
            info.AddValue("crawlThreads", this.crawlThreads);
            info.AddValue("processThreads", this.processThreads);
            info.AddValue("proxy", this.proxy,typeof(IWebProxy));
            info.AddValue("userAgent", this.userAgent);
            info.AddValue("referer", this.referer);
            info.AddValue("cookies", this.cookies,typeof(CookieCollection));
            info.AddValue("requestEncoding", this.requestEncoding,typeof(Encoding));
            info.AddValue("urlExtractor", this.urlExtractor,typeof(UrlExtractor));
            info.AddValue("contentHandlers", this.contentHandlers, typeof(ICollection<IContentHandler>));
            info.AddValue("logger", this.logger, typeof(ILogger));
            info.AddValue("memLimitSize", this.memLimitSize);
            info.AddValue("depositePath", this.depositePath);
            info.AddValue("speedMode", this.speedMode, typeof(SpiderSetting.SpeedModes));
        }

        #endregion
    }
}
