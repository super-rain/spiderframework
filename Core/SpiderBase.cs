/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// Spider基类
    /// </summary>
    public abstract class SpiderBase
    {
        #region SpiderRunStatus
        /// <summary>
        /// 运行状态的枚举
        /// </summary>
        [Serializable]
        public enum SpiderRunStatus : byte
        {
            UnStarted = 0,
            Initializing = 0x01,
            Running = 0x02,
            Stopped = 0x08
        }
        #endregion

        #region fields

        private SpiderSetting settings;             //爬虫配置
        private UrlManager urlManager;              //URL管理器
        private UrlQueue urlQueue;                  //URL队列
        private ContentQueue contentQueue;          //内容队列

        private Thread managerThread;               //一个守护线程,用于维护请求处理线程和内容处理线程

        private List<Thread> requestThreads;        //用于处理请求的线程集合
        private List<Thread> processThreads;        //用于处理内容的线程集合

        private volatile SpiderRunStatus runStatus; //运行状态
        private volatile bool pauseCalled;          //指示是否暂停
        private volatile bool completed;            //指示采集和处理线程是否全部完成

        private DateTime startTime;                 //开始时间
        private TimeSpan startTimespan;             //开始计时的时刻

        private SpiderRuntime runtime;              //运行时信息

        #endregion


        #region constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        private SpiderBase()
        {
            this.urlQueue = new UrlQueue();
            this.contentQueue = new ContentQueue();

            this.requestThreads = new List<Thread>();
            this.processThreads = new List<Thread>();

            this.runStatus = SpiderRunStatus.UnStarted;
            this.pauseCalled = false;
            this.completed = false;

            this.startTimespan = new TimeSpan();
            this.startTime = DateTime.MinValue;

            this.runtime = new SpiderRuntime();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="st">SpiderSetting</param>
        /// <param name="urlMgr">UrlManager</param>
        public SpiderBase(SpiderSetting st, UrlManager urlMgr)
            : this()
        {
            this.settings = st;
            this.urlManager = urlMgr;
        }

        #endregion


        #region properties

        /// <summary>
        /// 获取当前的配置信息
        /// </summary>
        public SpiderSetting Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <summary>
        /// 获取当前的待处理的Url队列
        /// </summary>
        protected UrlQueue UrlQueue
        {
            get
            {
                return this.urlQueue;
            }
        }

        /// <summary>
        /// 获取当前的待处理的内容队列
        /// </summary>
        protected ContentQueue ContentQueue
        {
            get
            {
                return this.contentQueue;
            }
        }

        /// <summary>
        /// 获取当前的URL管理器
        /// </summary>
        protected UrlManager UrlManager
        {
            get
            {
                return this.urlManager;
            }
        }

        /// <summary>
        /// 获取最近一次启动时间
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
        }

        /// <summary>
        /// 获取运行时信息
        /// </summary>
        public SpiderRuntime Runtime
        {
            get
            {
                this.runtime.crawlThreads = this.requestThreads.Count;
                this.runtime.processThreads = this.processThreads.Count;
                this.runtime.urlQueueLength = this.urlQueue.Count;
                this.runtime.contentQueueLength = this.contentQueue.Count;
                this.runtime.urlCount = this.urlManager.Count;

                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                this.runtime.seconds = ts.Subtract(this.startTimespan).TotalSeconds;

                return this.runtime;
            }
        }


        /// <summary>
        /// 获取一个值,指示当前工作是否已全部完成
        /// </summary>
        public bool Completed
        {
            get
            {
                return this.completed;
            }
        }

        /// <summary>
        /// 获取当前的运行状态
        /// </summary>
        public SpiderRunStatus Status
        {
            get
            {
                return this.runStatus;
            }
        }

        #endregion


        #region public methods

        /// <summary>
        /// 启动爬虫
        /// </summary>
        public void Start()
        {
            if (this.runStatus != SpiderRunStatus.UnStarted)
            {
                Console.WriteLine("Spider was started,and you can not restart it again!");
                return;
            }

            if (Environment.UserInteractive)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("##########################################################################");
                Console.WriteLine("##                                                                      ##");
                Console.WriteLine("##              SpiderLib, Author:mickey_xu@brainet.com.cn              ##");
                string _title = this.settings.Name;
                if (_title.Length > 60)
                {
                    _title = _title.Substring(0, 60) + "......";
                }
                int _len = _title.Length;
                if (_len % 2 == 0)
                {
                    _len += (70 - _len) / 2;
                }
                else
                {
                    _len += (70 - _len - 1) / 2;
                }
                _title = _title.PadLeft(_len, ' ').PadRight(70, ' ');
                Console.WriteLine("##{0}##", _title);
                Console.WriteLine("##                                                                      ##");
                Console.WriteLine("##########################################################################");
                Console.WriteLine("");
                Console.ResetColor();
            }

            //执行开始之前的操作
            this.BeforeStarting();

            if (null != this.settings.StartUrl)
            {
                this.urlQueue.Add(this.settings.StartUrl);
            }

            Thread initThread = new Thread(new ThreadStart(this.urlManager.Init));
            initThread.Start();
            this.runStatus = SpiderRunStatus.Initializing;

            //执行初始化阶段的操作
            this.OnInitializing();

            //等待初始化线程完成
            while (!initThread.IsAlive) ;
            initThread.Join();

            //执行初始化完成后的操作
            this.OnInitialized();

            //Manage Thread
            this.managerThread = new Thread(new ThreadStart(this.ManageThreads));
            this.managerThread.IsBackground = true;
            this.managerThread.Name = "ManagerThread";

            this.runStatus = SpiderRunStatus.Running;
            this.managerThread.Start();

            //阻塞,等待ManagerThread启动
            while (!this.managerThread.IsAlive) ;

            //开始时刻
            this.startTime = DateTime.Now;
            this.startTimespan = new TimeSpan(this.startTime.Ticks);

            //执行启动时刻的操作
            this.OnStarted();
        }

        /// <summary>
        /// 手动排队(Manual Queue)URL
        /// </summary>
        /// <param name="urls">Url实例的枚举</param>
        public void ManualQueue(IEnumerable<Url> urls, bool allowExtract)
        {
            if (null == urls)
            {
                return;
            }

            using (IEnumerator<Url> enu = urls.GetEnumerator())
            {
                while (enu.MoveNext())
                {
                    this.ManualQueue(enu.Current, allowExtract);
                }
            }
        }

        /// <summary>
        /// 手动排队Content
        /// </summary>
        /// <param name="urls">Content集合</param>
        public void ManualQueue(IEnumerable<Content> contents)
        {
            if (null == contents)
            {
                return;
            }

            using (IEnumerator<Content> enu = contents.GetEnumerator())
            {
                while (enu.MoveNext())
                {
                    this.ManualQueue(enu.Current);
                }
            }
        }

        /// <summary>
        /// 手动排队URL
        /// </summary>
        /// <param name="url">URL</param>
        public void ManualQueue(Url url, bool allowExtract)
        {
            if (null == url)
            {
                return;
            }
            if (!allowExtract)
            {
                url.AllowExtractUrl = false;
            }
            this.urlQueue.Add(url);
        }

        /// <summary>
        /// 手动排队Content
        /// </summary>
        /// <param name="content">Content</param>
        public void ManualQueue(Content content)
        {
            if (null == content)
            {
                return;
            }
            this.contentQueue.Add(content);
        }

        /// <summary>
        /// 获取给定URL的内容,失败时返回NULL
        /// </summary>
        /// <param name="url">Url实例</param>
        /// <returns>Content实例或NULL</returns>
        private Content GetContent(Url url)
        {
            if (null == url)
            {
                return null;
            }
            string s = url.GetUrl();
            HttpWebRequest request = WebRequest.Create(s) as HttpWebRequest;
            this.settings.PrepareRequest(request);

            //带参数的POST请求
            if (url.HttpMethod == "POST" && null != url.AppendParams && url.AppendParams.Count > 0)
            {
                request.Method = "POST";
                request.ContentType = String.Format("application/x-www-form-urlencoded;charset={0}", this.settings.RequestEncoding.WebName);
                byte[] data = GetRequestData(url.AppendParams, this.settings.RequestEncoding);
                request.ContentLength = data.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                }
                this.runtime.bytesSent += data.Length;
            }

            HttpWebResponse response = null;
            Content content = null;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException we)
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine(String.Format("WebException状态:{0}, 消息:{1}", we.Status, we.Message));
                }

                url.SetError(we.Message);

                if (we.Status == WebExceptionStatus.Timeout || we.Status == WebExceptionStatus.ReceiveFailure || we.Status == WebExceptionStatus.UnknownError)
                {
                    //普通或者慢速模式下,进行失败重试
                    if (((int)this.settings.SpeedMode) >= ((int)SpiderSetting.SpeedModes.Normal) && url.MaxTryTimes > 0 && url.CanTryAgain)
                    {
                        url.AddTryTime();
                        this.urlQueue.Add(url);
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                if (Environment.UserInteractive)
                {
                    this.WriteLog(e.Message);
                }
                return null;
            }
            finally
            {
                this.runtime.urlTotal++;
            }

            if (null == response)
            {
                return null;
            }

            url.ClearError();

            using (Stream stream = response.GetResponseStream())
            {
                RequestContext context = new RequestContext(this.settings, url, response);

                content = Content.Create(context);

                if (null != content)
                {
                    content.Read(stream);
                    this.runtime.bytesLoaded += content.ContentLength;
                }
            }

            return content;
        }

        /// <summary>
        /// 获取给定URL的内容,失败时返回NULL
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="requestEncoding">请求编码</param>
        /// <param name="userAgent">User-Agent头</param>
        /// <param name="cookies">Cookies</param>
        /// <param name="proxy">网络代理</param>
        /// <param name="referer">URL-Referer头</param>
        /// <param name="requestTimeout">请求超时</param>
        /// <param name="iOTimeout">读写超时</param>
        /// <param name="allowRedirect">跳转设置,大于0表示允许跳转此值所表示的次数,否则不允许跳转</param>
        /// <returns>Content or NULL</returns>
        public static Content GetContent(Url url, Encoding requestEncoding, string userAgent, CookieContainer cookies, IWebProxy proxy, string referer, int requestTimeout, int iOTimeout, int allowRedirect)
        {
            if (null == url)
            {
                return null;
            }

            HttpWebRequest request = WebRequest.Create(url.GetUrl()) as HttpWebRequest;

            request.UserAgent = userAgent;
            if (null != cookies)
            {
                request.CookieContainer = cookies;
            }
            if (null != proxy)
            {
                request.Proxy = proxy;
            }
            if (!String.IsNullOrEmpty(referer))
            {
                request.Referer = referer;
            }
            if (requestTimeout > 0)
            {
                request.Timeout = requestTimeout;
            }
            if (iOTimeout > 0)
            {
                request.ReadWriteTimeout = iOTimeout;
            }
            if (allowRedirect > 0)
            {
                request.AllowAutoRedirect = true;
                request.MaximumAutomaticRedirections = allowRedirect;
            }

            //带参数的POST请求
            if (url.HttpMethod == "POST" && null != url.AppendParams && url.AppendParams.Count > 0)
            {
                request.Method = "POST";
                request.ContentType = String.Format("application/x-www-form-urlencoded;charset={0}", requestEncoding.WebName);
                byte[] data = GetRequestData(url.AppendParams, requestEncoding);
                request.ContentLength = data.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                }
            }

            HttpWebResponse response = null;
            Content content = null;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (Exception e)
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine(e);
                }
                return null;
            }

            if (null == response)
            {
                return null;
            }

            url.ClearError();

            using (Stream stream = response.GetResponseStream())
            {
                RequestContext context = new RequestContext(null, url, response);

                content = Content.Create(context);

                if (null != content)
                {
                    content.Read(stream);
                }
            }

            return content;
        }

        /// <summary>
        /// 获取给定URL的内容,失败时返回NULL
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="requestEncoding">请求编码</param>
        /// <param name="userAgent">User-Agent头</param>
        /// <returns>Content or NULL</returns>
        public static Content GetContent(Url url, Encoding requestEncoding, string userAgent)
        {
            return GetContent(url, requestEncoding, userAgent, null, null, null, 0, 0, 0);
        }

        /// <summary>
        /// 获取给定URL的内容,失败时返回NULL
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="requestEncoding">请求编码</param>
        /// <param name="userAgent">User-Agent头</param>
        /// <param name="cookies">Cookies</param>
        /// <returns>Content or NULL</returns>
        public static Content GetContent(Url url, Encoding requestEncoding, string userAgent, CookieContainer cookies)
        {
            return GetContent(url, requestEncoding, userAgent, cookies, null, null, 0, 0, 0);
        }

        /// <summary>
        /// 获取给定URL的内容,失败时返回NULL
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="requestEncoding">请求编码</param>
        /// <returns>Content or NULL</returns>
        public static Content GetContent(Url url, Encoding requestEncoding)
        {
            return GetContent(url, requestEncoding, SpiderSetting.DefaultUserAgent, null, null, null, 0, 0, 0);
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        public void Stop()
        {
            this.ExecStopping(false);
        }

        /// <summary>
        /// 暂停采集
        /// </summary>
        public void Pause()
        {
            this.pauseCalled = true;
            this.WriteLog("Paused,Waitting for a RESUME action!");
        }

        /// <summary>
        /// 从暂停恢复
        /// </summary>
        public void Resume()
        {
            this.pauseCalled = false;
            this.WriteLog("Resumed,and Spider is Running......");
        }

        #endregion


        #region protected virtual methods

        /// <summary>
        /// 在控制台模式下，设置控制台窗口标题
        /// </summary>
        protected virtual void SetConsoleTitle()
        {
            if (!Environment.UserInteractive)
            {
                return;
            }

            Console.Title = this.settings.Name + " - " + this.Runtime.ToString();
        }

        /// <summary>
        /// 在派生类中重写此方法,以便在准备开始之前进行处理
        /// </summary>
        /// <param name="setting">SpiderSetting</param>
        protected virtual void BeforeStarting()
        {
            //
        }

        /// <summary>
        /// 在派生类中重写此方法,以便在启动后的第一时刻进行处理
        /// </summary>
        protected virtual void OnStarted()
        {
            //
        }

        /// <summary>
        /// 在派生类中重写此方法,以便在进行初始化期间进行处理
        /// </summary>
        /// <param name="setting">SpiderSetting</param>
        protected virtual void OnInitializing()
        {
            //
        }

        /// <summary>
        /// 在派生类中重写此方法,以便在进行初始化完成后,启动之前进行处理
        /// </summary>
        protected virtual void OnInitialized()
        {
            //
        }

        /// <summary>
        /// 在派生类中重写此方法,以便在停止之前进行处理
        /// </summary>
        protected virtual void BeforeStopped(bool isStoppedByManual)
        {
            //
        }

        /// <summary>
        /// 在派生类中重写此方法,以便在停止之后进行处理
        /// </summary>
        protected virtual void AfterStopped()
        {
            //
        }


        /// <summary>
        /// 在派生类中重写此方法,以便在排队一个URL之前进行处理
        /// </summary>
        /// <param name="url">Url</param>
        protected virtual void BeforeEnqueueUrl(Url url)
        {
            //
        }

        /// <summary>
        /// 在派生类中重写此方法,以便在排队一个Content之前进行处理
        /// </summary>
        /// <param name="content">Content</param>
        protected virtual void BeforeEnqueueContent(Content content)
        {
            //
        }

        /// <summary>
        /// 在派生类中重写此方法,以便在Content下载完成进行处理
        /// </summary>
        /// <param name="content"></param>
        protected virtual void OnContentLoaded(Content content)
        {
            //
        }


        /// <summary>
        /// 在派生类中重写此方法,以便在完成对一个Content处理之后进行处理
        /// </summary>
        /// <param name="content">Content</param>
        /// <param name="context">ProcessContext</param>
        protected virtual void OnProcessCompleted(Content content, ProcessContext context)
        {
            //
        }

        #endregion

        #region private methods

        /// <summary>
        /// 管理请求处理线程和内容处理线程
        /// </summary>
        private void ManageThreads()
        {
            while (this.runStatus == SpiderRunStatus.Running)
            {
                while (this.pauseCalled)
                {
                    Thread.Sleep(5);
                }

                SetConsoleTitle();

                //完成,进行最后的处理
                if (this.urlQueue.Count == 0 && this.contentQueue.Count == 0 && this.requestThreads.Count == 0 && this.processThreads.Count == 0)
                {
                    this.completed = true;
                    Thread t = new Thread(new ParameterizedThreadStart(this.ExecStopping));
                    t.Start(false);
                    while (!t.IsAlive) ;
                    t.Join();
                    break;
                }

                //维护采集线程
                //new Thread(new ParameterizedThreadStart(this.DestroyThreads)).Start(this.requestThreads);
                this.DestroyThreads(this.requestThreads);

                int n = this.urlQueue.Count;
                if (n > 0 && this.requestThreads.Count < this.settings.CrawlThreads)
                {
                    Thread t1 = new Thread(new ThreadStart(this._DoRequest));
                    t1.IsBackground = true;
                    t1.Name = "T_CRAWL";
                    this.requestThreads.Add(t1);
                    t1.Start();
                }

                //维护处理线程
                //new Thread(new ParameterizedThreadStart(this.DestroyThreads)).Start(this.processThreads);
                this.DestroyThreads(this.processThreads);

                int m = this.contentQueue.Count;
                if (m > 0 && this.processThreads.Count < this.settings.ProcessThreads)
                {
                    Thread t2 = new Thread(new ThreadStart(this._DoProcess));
                    t2.IsBackground = true;
                    this.processThreads.Add(t2);
                    t2.Name = "T_PROCE";
                    t2.Start();
                }

                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// 阻塞等待线程结束,并从线程列表中清除
        /// </summary>
        /// <param name="threadList">线程列表</param>
        private void DestroyThreads(List<Thread> threadList)
        {
            for (int i = 0; i < threadList.Count; i++)
            {
                if (!threadList[i].IsAlive)
                {
                    threadList[i].Join();
                    threadList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 执行停止前的操作
        /// </summary>
        private void ExecStopping(object manual)
        {
            //执行停止之前的操作
            this.BeforeStopped((bool)manual);

            this.runStatus = SpiderRunStatus.Stopped;

            //销毁线程
            this.DestroyThreads(this.requestThreads);
            this.DestroyThreads(this.processThreads);

            SetConsoleTitle();

            //执行停止之后的操作
            this.AfterStopped();
        }

        /// <summary>
        /// 发送请求,从队列Dequeue
        /// </summary>
        private void _DoRequest()
        {
            if (this.runStatus != SpiderRunStatus.Running)
            {
                return;
            }

            Url url = this.urlQueue.Next();
            if (null == url)
            {
                return;
            }

            this.DoRequest(url);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url">Url</param>
        private void DoRequest(Url url)
        {
            this.urlManager.AddUrl(url);

            Content content = null;

            try
            {
                content = this.GetContent(url);
            }
            catch (Exception e)
            {
                this.WriteLog(e);
            }

            if (null == content)
            {
                return;
            }

            //当下载一个Content完成时操作
            this.OnContentLoaded(content);

            //将最终URL的内容加入内容处理的队列
            if (url.GetType() == typeof(FinalUrl))
            {
                //在排队一个Content之前操作
                this.BeforeEnqueueContent(content);

                this.contentQueue.Add(content);

                //不从最终页面抽取URL
                if (!this.settings.UrlExtractor.ExtractFinal)
                {
                    return;
                }
            }


            //提取URL
            if (null != this.settings.UrlExtractor && url.AllowExtractUrl)
            {
                IDictionary<uint, Url> urls = this.settings.UrlExtractor.Extract(content);
                foreach (uint key in urls.Keys)
                {
                    if (key != this.settings.StartUrl.CheckSum && !this.urlManager.IsExistsKey(key))
                    {
                        //在排队一个URL之前操作
                        this.BeforeEnqueueUrl(urls[key]);

                        this.urlQueue.Add(urls[key]);
                    }
                }
            }

            Thread.Sleep((int)this.settings.SpeedMode);
        }


        /// <summary>
        /// 准备随同请求一起发送的数据,返回数据的字节长度
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="args">NameValueCollection</param>
        /// <returns>int</returns>
        internal static byte[] GetRequestData(NameValueCollection requestArgs, Encoding requestEncoding)
        {
            if (null == requestArgs || null == requestArgs || requestArgs.Count == 0)
            {
                return new byte[0];
            }

            string[] dataString = new string[requestArgs.Count];
            for (int i = 0; i < requestArgs.Count; i++)
            {
                dataString[i] = String.Format("{0}={1}", requestArgs.Keys[i], requestArgs.Get(i));
            }
            byte[] data = requestEncoding.GetBytes(String.Join("&", dataString));
            return data;
        }

        /// <summary>
        /// 处理内容,从队列Dequeue
        /// </summary>
        private void _DoProcess()
        {
            if (this.runStatus != SpiderRunStatus.Running)
            {
                return;
            }

            Content content = this.contentQueue.Next();
            if (null == content)
            {
                return;
            }

            this.DoProcess(content);
        }

        /// <summary>
        /// 处理内容
        /// </summary>
        /// <param name="content">Content实例</param>
        private void DoProcess(Content content)
        {
            if (null == this.settings.ContentHandlers && null == content.RawUrl.ContentHandlers)
            {
                return;
            }

            if (null == content)
            {
                return;
            }

            //排序处理程序集合
            List<IContentHandler> handlers = new List<IContentHandler>();
            foreach (IContentHandler h in this.settings.ContentHandlers)
            {
                if (null != h)
                {
                    handlers.Add(h);
                }
            }

            foreach (IContentHandler h in content.RawUrl.ContentHandlers)
            {
                if (null != h)
                {
                    handlers.Add(h);
                }
            }

            handlers.Sort(new ContentHandlerPriorityCompare());

            ProcessContext context = new ProcessContext(this);

            using (content)
            {
                //按照优先次序调用内容处理程序
                foreach (IContentHandler handler in handlers)
                {
                    handler.Process(content, context);
                }

                //处理完成后操作
                this.OnProcessCompleted(content, context);
            }

            Thread.Sleep((int)this.settings.SpeedMode);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="message"></param>
        private void WriteLog(object message)
        {
            if (null == this.settings.Logger)
            {
                return;
            }
            this.settings.Logger.WriteLog(message);
        }

        #endregion
    }
}
