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
using System.Web;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示一个URL抽象基类
    /// </summary>
    public abstract class Url : ISerializable
    {
        #region consts
        internal const int MaxTextLength = 100;                 //允许的URL文本内容最大长度
        internal const int MaxTryRequestTimes = 5;              //最多允许的重试次数常量
        #endregion

        #region private fields

        private Uri uri;                                        //Uri
        private bool uriEscape;                                 //指示是否对Uri进行转义
        private UInt32 checkSum;                                //校验和
        private UInt32 hostCheckSum;                            //主机部分的校验和

        private IUrlCheckSum checkSumAlgorith;                  //校验和算法接口
        private string httpMethod;                              //请求方法,POST或GET,默认GET

        private NameValueCollection appendParams;               //附加参数
        private NameValueCollection ignoreParams;               //计算校验和时要忽略的参数
        private int maxTryTimes;                                //允许的最大失败重试次数
        private int hasTriedTimes;                              //已经重试的次数
        private bool hasError;                                  //指示是否发生错误
        private string errorMsg;                                //最后一个错误消息
        private ContentHandlerCollection contentHandlers;   //内容处理程序接口集合,这些处理程序会按照优先次序被依次执行

        private NameValueCollection rawParams;                  //当前URL的原始参数集合
        private string text;                                    //当前链接的文本内容,对于Img,为alt描述文本年

        private string domain;                                  //当前URL的域部分

        private IDomainSuffixPrivoder domainSuffixProvider;     //常见域名后缀提供程序接口

        private bool allowExtractUrl;

        #endregion

        #region constructors
        /// <summary>
        /// 构造函数
        /// </summary>
        private Url()
        {
            this.uri = null;
            this.uriEscape = true;
            this.checkSum = UInt32.MinValue;
            this.hostCheckSum = UInt32.MinValue;
            this.checkSumAlgorith = new GeneralUrlCheckSum(true);
            this.httpMethod = "GET";
            this.appendParams = null;
            this.maxTryTimes = 1;
            this.hasTriedTimes = 0;
            this.hasError = false;
            this.errorMsg = "";
            this.text = "";

            this.contentHandlers = new ContentHandlerCollection();

            this.domain = null;
            this.domainSuffixProvider = Core.DomainSuffixProvider.Default;

            this.allowExtractUrl = true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseUrl">绝对基URL的字符串表示</param>
        /// <param name="relativeUrl">相对于基URL的URL的字符串表示</param>
        protected Url(string baseUrl, string relativeUrl)
            : this()
        {
            baseUrl = Utils.AmendUrlString(baseUrl);
            if (String.IsNullOrEmpty(relativeUrl))
            {
                this.uri = new Uri(baseUrl, UriKind.Absolute);
            }
            else
            {
                this.uri = new Uri(new Uri(baseUrl, UriKind.Absolute), relativeUrl);
            }

            this.DoCheckSum(true);
        }

        protected Url(SerializationInfo info,StreamingContext context)
        {
            this.uri = info.GetValue("uri", typeof(Uri)) as Uri;
            this.uriEscape = info.GetBoolean("escape");
            this.checkSum = info.GetUInt32("checkSum");
            this.hostCheckSum = info.GetUInt32("hostCheckSum");
            this.checkSumAlgorith = info.GetValue("checkSumAlgorith", typeof(IUrlCheckSum)) as IUrlCheckSum;
            this.httpMethod = info.GetString("httpMethod");
            this.appendParams = info.GetValue("appendParams", typeof(NameValueCollection)) as NameValueCollection;
            this.ignoreParams = info.GetValue("ignoreParams", typeof(NameValueCollection)) as NameValueCollection;
            this.maxTryTimes = info.GetInt32("maxTryTimes");
            this.hasTriedTimes = info.GetInt32("hasTriedTimes");
            this.hasError = info.GetBoolean("hasError");
            this.errorMsg = info.GetString("errorMsg");
            this.contentHandlers = info.GetValue("contentHandlers", typeof(ContentHandlerCollection)) as ContentHandlerCollection;
            this.text = info.GetString("text");
            this.domainSuffixProvider = info.GetValue("domainSuffixProvider", typeof(IDomainSuffixPrivoder)) as IDomainSuffixPrivoder;
            this.allowExtractUrl = info.GetBoolean("allowExtractUrl");
        }

        #endregion


        #region public properties

        /// <summary>
        /// 获取当前URL的Uri
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.uri;
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示是否对URL字符串进行转义,默认为true
        /// </summary>
        public bool UriEscape
        {
            get
            {
                return this.uriEscape;
            }
            set
            {
                this.uriEscape = value;
            }
        }

        /// <summary>
        /// 获取或设置URL的文本描述内容
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                if (this.text != null && this.text.Length > MaxTextLength)
                {
                    this.text = this.text.Substring(0, MaxTextLength);
                }
            }
        }

        /// <summary>
        /// 获取或设置当前URL的CheckSum算法
        /// </summary>
        public IUrlCheckSum CheckSumAlgorith
        {
            get
            {
                return this.checkSumAlgorith;
            }
            set
            {
                this.checkSumAlgorith = value;
                this.DoCheckSum(false);
            }
        }

        /// <summary>
        /// 获取当前URL的校验和
        /// </summary>
        public UInt32 CheckSum
        {
            get
            {
                return this.checkSum;
            }
        }

        /// <summary>
        /// 获取当前URL主机部分(Uri.Host)的校验和
        /// </summary>
        public UInt32 HostCheckSum
        {
            get
            {
                return this.hostCheckSum;
            }
        }

        /// <summary>
        /// 获取或设置此URL的访问方法,GET或POST
        /// </summary>
        public string HttpMethod
        {
            get
            {
                return this.httpMethod;
            }
            set
            {
                this.httpMethod = (value == "POST" ? value : "GET");
            }
        }

        /// <summary>
        /// 获取一个值,指示当前URL是否经过检验有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.uri.Host.Length > 1 && this.checkSum > 0;
            }
        }


        /// <summary>
        /// 获取或设置附加的请求参数
        /// </summary>
        public NameValueCollection AppendParams
        {
            get
            {
                return this.appendParams;
            }
            set
            {
                this.appendParams = value;
            }
        }

        /// <summary>
        /// 获取或设置要忽略的参数,被忽略的参数会随请求一起发送,但是不会被计算到校验和中
        /// </summary>
        public NameValueCollection IgnoreParams
        {
            get
            {
                return this.ignoreParams;
            }
            set
            {
                this.ignoreParams = value;

                //重新计算CheckSum
                this.CheckSumAlgorith.IgnoreParmasCollection = this.ignoreParams;
                this.DoCheckSum(false);
            }
        }

        /// <summary>
        /// 获取当前URL的域部分
        /// </summary>
        /// <returns></returns>
        public string Domain
        {
            get
            {
                if (null == this.domain)
                {
                    this.domain = GetUrlDomain(this, this.domainSuffixProvider);
                }
                return this.domain;
            }
        }

        /// <summary>
        /// 获取当前URL类型,UrlTypes枚举值之一
        /// </summary>
        public UrlTypes UrlType
        {
            get
            {
                if (this.GetType() == typeof(StartUrl) || this.GetType() == typeof(IndexUrl))
                {
                    return UrlTypes.Index;
                }
                return UrlTypes.Final;
            }
        }

        /// <summary>
        /// 获取或设置允许的最大重试请求次数,当一次请求失败后会检查此项,设置为0将不进行重试
        /// </summary>
        public int MaxTryTimes
        {
            get
            {
                return this.maxTryTimes;
            }
            set
            {
                this.maxTryTimes = value;
                if (this.maxTryTimes > MaxTryRequestTimes)
                {
                    this.maxTryTimes = MaxTryRequestTimes;
                }
            }
        }

        /// <summary>
        /// 获取一个值,指示当前URL的请求是否发生错误
        /// </summary>
        public bool HasError
        {
            get
            {
                return this.hasError;
            }
        }

        /// <summary>
        /// 获取最后一个错误消息,没有错误则返回空字符串
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this.hasError ? this.errorMsg : "";
            }
        }

        /// <summary>
        /// 获取一个值,指示当前URL是否仍然可以进行请求重试
        /// </summary>
        /// <returns>bool</returns>
        public bool CanTryAgain
        {
            get
            {
                return this.maxTryTimes > 0 && this.hasTriedTimes < this.maxTryTimes;
            }
        }

        /// <summary>
        /// 获取当前URL定义的处理程序集合
        /// </summary>
        public ICollection<IContentHandler> ContentHandlers
        {
            get
            {
                return this.contentHandlers;
            }
        }

        /// <summary>
        /// 获取或设置域名后缀提供者
        /// </summary>
        public IDomainSuffixPrivoder DomainSuffixProvider
        {
            get
            {
                return this.domainSuffixProvider;
            }
            set
            {
                this.domainSuffixProvider = value;
            }
        }

        internal bool AllowExtractUrl
        {
            get
            {
                return this.allowExtractUrl;
            }
            set
            {
                this.allowExtractUrl = value;
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// 计算当前URL的校验和
        /// </summary>
        /// <param name="host">指示是否计算Host部分的校验和</param>
        /// <returns>int</returns>
        private void DoCheckSum(bool host)
        {
            if (null == this.uri)
            {
                return;
            }

            this.checkSum = this.CheckSumAlgorith.CheckSum(this);

            if (host)
            {
                this.hostCheckSum = CRC32.CheckSum(this.uri.Host);
            }
        }

        #endregion

        #region internal methods

        /// <summary>
        /// 设置URL的请求时错误
        /// </summary>
        /// <param name="err">有无错误</param>
        /// <param name="message">错误消息</param>
        internal void SetError(string message)
        {
            this.hasError = true;
            this.errorMsg = (string)message;
        }

        /// <summary>
        /// 清除错误
        /// </summary>
        internal void ClearError()
        {
            this.hasError = false;
            this.errorMsg = "";
        }

        /// <summary>
        /// 增加一次请求次数
        /// </summary>
        internal void AddTryTime()
        {
            this.hasTriedTimes++;
        }

        #endregion

        #region public methods

        public string GetUrl()
        {
            if (this.uriEscape)
            {
                return this.uri.AbsoluteUri;
            }

            return this.uri.OriginalString;
        }

        /// <summary>
        /// 获取URL指定名称的参数值,无匹配项时返回NULL
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="enc">指定编码</param>
        /// <returns>string</returns>
        public string GetRawParam(string name, Encoding enc)
        {
            if (null == this.rawParams)
            {
                this.rawParams = HttpUtility.ParseQueryString(this.uri.Query, enc);
            }
            return this.rawParams.Get(name);
        }

        /// <summary>
        /// 使用系统默认的编码(Encoding.Default)获取URL指定名称的参数值,无匹配项时返回NULL
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns>string</returns>
        public string GetRawParam(string name)
        {
            return this.GetRawParam(name, Encoding.Default);
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return String.Format("{0}, {1} <{2}>:[{3}]", this.checkSum, this.uri.AbsoluteUri, this.text, this.hasTriedTimes);
        }


        /// <summary>
        /// 通过比较两个URL的CheckSum值来判断是否相等
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>bool</returns>
        public new bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Url))
            {
                return false;
            }
            return this.checkSum == ((Url)obj).checkSum;
        }

        #endregion

        #region private static methods

        /// <summary>
        /// 从给定的绝对URL和类型创建URL实例
        /// </summary>
        /// <param name="url">绝对URL字符串的表示</param>
        /// <param name="type">UrlTypes</param>
        /// <returns>Url</returns>
        public static Url CreateIndexUrl(string url)
        {
            return Create(url, "", UrlTypes.Index, null);
        }

        /// <summary>
        /// 从给定的绝对URL和类型创建URL实例
        /// </summary>
        /// <param name="url">绝对URL字符串的表示</param>
        /// <param name="type">UrlTypes</param>
        /// <returns>Url</returns>
        public static Url CreateIndexUrl(string url, string relative)
        {
            return Create(url, relative, UrlTypes.Index, null);
        }


        /// <summary>
        /// 从给定的绝对URL和类型创建URL实例
        /// </summary>
        /// <param name="url">绝对URL字符串的表示</param>
        /// <param name="type">UrlTypes</param>
        /// <returns>Url</returns>
        public static Url CreateFinalUrl(string url, Url holder)
        {
            return Create(url, "", UrlTypes.Final, null);
        }

        /// <summary>
        /// 从给定的绝对URL和类型创建URL实例
        /// </summary>
        /// <param name="url">绝对URL字符串的表示</param>
        /// <param name="relative">relative url</param>
        /// <param name="holder">持有URL</param>
        /// <returns>Url</returns>
        public static Url CreateFinalUrl(string url, string relative, Url holder)
        {
            return Create(url, relative, UrlTypes.Final, null);
        }


        /// <summary>
        /// 从给定的绝对URL和相对URL,以及类型创建URL实例
        /// </summary>
        /// <param name="url">绝对URL字符串的表示</param>
        /// <param name="relativeUrl">相对对URL字符串的表示</param>
        /// <param name="type">UrlTypes</param>
        /// <param name="holder">holder URL</param>
        /// <returns>Url</returns>
        private static Url Create(string url, string relativeUrl, UrlTypes type, Url holder)
        {
            Url u = null;
            switch (type)
            {
                case UrlTypes.Index:
                    u = new IndexUrl(url, relativeUrl);
                    break;

                case UrlTypes.Final:
                    u = new FinalUrl(url, relativeUrl, holder);
                    break;
            }
            return u;
        }

        #endregion

        #region public static methods

        /// <summary>
        /// 比较两个URL的主机部分是否相同,如host1.domain.com和host1.domain.com的主机部分相同,host1.domain.com和host2.domain.com则不同
        /// </summary>
        /// <param name="url1">Url</param>
        /// <param name="url2">Url</param>
        /// <returns>bool</returns>
        public static bool IsSameHost(Url url1, Url url2)
        {
            if (null == url1 || null == url2)
            {
                return false;
            }
            return url1.HostCheckSum == url2.HostCheckSum;
        }

        /// <summary>
        /// 比较两个URL的域部分是否相同,如a.domain.com和b.domain.com的域部分相同,a.domain1.com和a.domain2.com则不同
        /// </summary>
        /// <param name="url1">Url</param>
        /// <param name="url2">Url</param>
        /// <returns>bool</returns>
        public static bool IsSameDomain(Url url1, Url url2)
        {
            if (!IsSameHost(url1, url2))
            {
                return false;
            }

            if ("" == url1.Domain || "" == url2.Domain)
            {
                return false;
            }
            return url1.Domain == url2.Domain;
        }

        /// <summary>
        /// 获取URL的域部分,如www.urbly.com的域部分为urbly.com,www2.test.urbly.com.cn的域部分为urbly.com.cn.
        /// </summary>
        /// <returns>string,无法获取时返回空字符串</returns>
        public static string GetUrlDomain(Url url, IDomainSuffixPrivoder provider)
        {
            if (null == url || null == provider)
            {
                return "";
            }

            string host = url.Uri.Host;
            string[] suffixes = provider.GetSuffixes();
            if (null == suffixes)
            {
                return "";
            }

            string pattern = String.Join("|", suffixes).Replace(".", "\\.");
            pattern = "\\.(?<D>[\\w-]+)(?:" + pattern + ")$";

            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(host);
            if (m.Success)
            {
                return host.Substring(m.Groups["D"].Index);
            }
            return "";
        }

        /// <summary>
        /// 比较两个URL是否相同
        /// </summary>
        /// <param name="url1">Url</param>
        /// <param name="url2">Url</param>
        /// <returns>bool</returns>
        public static bool IsEquals(Url url1, Url url2)
        {
            if (url1.GetType() != url2.GetType())
            {
                return false;
            }
            return url1.CheckSum == url2.CheckSum;
        }

        /// <summary>
        /// 比较两个URL的Path部分是否相同
        /// </summary>
        /// <param name="url1">Url</param>
        /// <param name="url2">Url</param>
        /// <returns>bool</returns>
        public static bool IsSamePath(Url url1, Url url2)
        {
            return url1.uri.AbsolutePath == url2.uri.AbsolutePath;
        }

        #endregion

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("uri", this.uri, typeof(Uri));
            info.AddValue("escape",this.uriEscape);
            info.AddValue("checkSum", this.checkSum);
            info.AddValue("hostCheckSum", this.hostCheckSum);
            info.AddValue("checkSumAlgorith", this.checkSumAlgorith,typeof(IUrlCheckSum));
            info.AddValue("httpMethod", this.httpMethod);
            info.AddValue("appendParams", this.appendParams,typeof(NameValueCollection));
            info.AddValue("ignoreParams", this.ignoreParams,typeof(NameValueCollection));
            info.AddValue("maxTryTimes", this.maxTryTimes);
            info.AddValue("hasTriedTimes", this.hasTriedTimes);
            info.AddValue("hasError", this.hasError);
            info.AddValue("errorMsg", this.errorMsg);
            info.AddValue("contentHandlers", this.contentHandlers, typeof(ContentHandlerCollection));
            info.AddValue("text", this.text);
            info.AddValue("domainSuffixProvider", this.domainSuffixProvider,typeof(IDomainSuffixPrivoder));
            info.AddValue("allowExtractUrl", this.allowExtractUrl);
        }

        #endregion
    }
}
