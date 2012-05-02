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
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示一个URL抽取器
    /// </summary>
    [Serializable]
    public abstract class UrlExtractor:ISerializable
    {
        /// <summary>
        /// 用于提取Img的alt属性的正则表达式,分组名:ALT
        /// </summary>
        public static Regex ImageAltRegex = new Regex("alt=\\s*('|\\\")?\\s*(?<ALT>[^'\\\"]+?)\\s*\\1", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// 用于提取A的href属性的正则表达式,URL分组名:URL,文本分组名:TEXT
        /// </summary>
        public static Regex AnchorHrefRegex = new Regex("<\\s*a[^>]+href=('|\\\")?\\s*(?!#|javascript:)(?<URL>[^>\\s]+?)\\s*\\1\\s*[^>]*>\\s*(?<TEXT>[^<>]+?)?\\s*<\\s*/a\\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        private UrlRulerCollection rulers; //用于抽取的规则
        private bool onlySameDomain;    //指示是否仅抽取相同域名的URL
        private bool onlySameHost;      //指示是否仅抽取相同主机的URL
        private bool extractFinal;      //指示是否在最终链接中进行URL抽取

        /// <summary>
        /// 构造函数
        /// </summary>
        protected UrlExtractor()
        {
            this.rulers = new UrlRulerCollection();
            this.onlySameDomain = true;
            this.onlySameHost = true;
            this.extractFinal = false;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected UrlExtractor(SerializationInfo info,StreamingContext context)
        {
            this.rulers = info.GetValue("rulers", typeof(UrlRulerCollection)) as UrlRulerCollection;
            this.onlySameDomain = info.GetBoolean("onlySameDomain");
            this.onlySameHost = info.GetBoolean("onlySameHost");
            this.extractFinal = info.GetBoolean("extractFinal");
        }

        /// <summary>
        /// 获取或设置一个值,指示是否仅提取与当前内容在同一域下的URL
        /// </summary>
        public bool OnlySameDomain
        {
            get
            {
                return this.onlySameDomain;
            }
            set
            {
                this.onlySameDomain = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值,指示是否仅提取与当前内容在同一主机下的URL
        /// </summary>
        public bool OnlySameHost
        {
            get
            {
                return this.onlySameHost;
            }
            set
            {
                this.onlySameHost = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值,指示是否从最终URL的内容中提取URL
        /// </summary>
        public bool ExtractFinal
        {
            get
            {
                return this.extractFinal;
            }
            set
            {
                this.extractFinal = value;
            }
        }

        /// <summary>
        /// 获取当前抽取器的所有规则列表
        /// </summary>
        public IList<UrlRuler> Rulers
        {
            get
            {
                return this.rulers;
            }
        }


        /// <summary>
        /// 在派生类中重写此方法,实现URL的抽取
        /// </summary>
        /// <param name="content">Content</param>
        /// <returns>IList</returns>
        public IDictionary<uint, Url> Extract(Content content)
        {
            IEnumerable<KeyValuePair<string, string>> strUrls = this.FindUrls(content);
            IDictionary<uint, Url> urls = new Dictionary<uint, Url>();

            if (null == strUrls)
            {
                return urls;
            }

            Url holder = content.RawUrl;
            Url url;
            UrlRuler matchedRuler;

            //遍历抽取到的URL
            using (IEnumerator<KeyValuePair<string, string>> enm = strUrls.GetEnumerator())
            {
                while (enm.MoveNext())
                {
                    matchedRuler = null;

                    //循环使用当前抽取器的URL规则来匹配当前的URL字符串,匹配成功则跳出循环,进一步处理
                    foreach (UrlRuler r in this.rulers)
                    {
                        if (r.IsMatch(enm.Current.Key))
                        {
                            matchedRuler = r;
                            break;
                        }
                    }

                    //进一步处理匹配到的URL
                    if (null != matchedRuler)
                    {
                        url = null;
                        try
                        {
                            switch (matchedRuler.UrlType)
                            {
                                case UrlTypes.Index:
                                    if (Utils.IsAbsoluteUrlString(enm.Current.Key))
                                    {
                                        url = Url.CreateIndexUrl(enm.Current.Key);
                                    }
                                    else
                                    {
                                        url = Url.CreateIndexUrl(content.RawUrl.Uri.AbsoluteUri, enm.Current.Key);
                                    }
                                    break;

                                case UrlTypes.Final:
                                    if (Utils.IsAbsoluteUrlString(enm.Current.Key))
                                    {
                                        url = Url.CreateFinalUrl(enm.Current.Key, holder);
                                    }
                                    else
                                    {
                                        url = Url.CreateFinalUrl(content.RawUrl.Uri.AbsoluteUri, enm.Current.Key, holder);
                                    }
                                    break;
                            }
                        }
                        catch (Exception e1)
                        {
                            Console.WriteLine("无法创建URL,错误:{0}, URL字符串:{1}", e1, enm.Current.Key);
                            continue;
                        }

                        //当URL无效,或者存在相同主机部分限制时
                        if (null == url || !url.IsValid || (this.OnlySameHost && !Url.IsSameHost(content.RawUrl, url)) || (this.OnlySameDomain && !Url.IsSameDomain(content.RawUrl, url)))
                        {
                            continue;
                        }

                        url.Text = enm.Current.Value;

                        //将匹配规则的属性赋予匹配的URL
                        url.HttpMethod = matchedRuler.HttpMethod;
                        url.AppendParams = matchedRuler.AppendParmas;
                        foreach (IContentHandler h in matchedRuler.ContentHandlers)
                        {
                            url.ContentHandlers.Add(h);
                        }

                        if (!urls.ContainsKey(url.CheckSum))
                        {
                            urls.Add(url.CheckSum, url);
                        }
                    }
                }
            }

            return urls;
        }

        /// <summary>
        /// 在派生类中重写此方法,从给定的内容中查找URL字符串
        /// </summary>
        /// <param name="content">Content</param>
        /// <returns>IEnumerable(string)</returns>
        protected abstract IEnumerable<KeyValuePair<string, string>> FindUrls(Content content);

        /// <summary>
        /// 从一组内容中进行URL抽取
        /// </summary>
        /// <param name="content">Content集合</param>
        /// <returns>IList</returns>
        public IDictionary<uint, Url> Extract(IList<Content> contents)
        {
            IDictionary<uint, Url> urls1 = new Dictionary<uint, Url>();
            IDictionary<uint, Url> urls2;
            foreach (Content c in contents)
            {
                urls2 = this.Extract(c);
                if (null != urls2)
                {
                    foreach (uint checksum in urls2.Keys)
                    {
                        if (!urls1.ContainsKey(checksum))
                        {
                            urls1.Add(checksum, urls2[checksum]);
                        }
                    }
                }
            }

            return urls1;
        }

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("rulers", this.rulers, typeof(UrlRulerCollection));
            info.AddValue("onlySameDomain", this.onlySameDomain);
            info.AddValue("onlySameHost", this.onlySameHost);
            info.AddValue("extractFinal", this.extractFinal);
        }

        #endregion
    }
}
