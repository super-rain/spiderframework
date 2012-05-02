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
using System.Net;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 为采集内容提供上下文信息
    /// </summary>
    [Serializable]
    public sealed class RequestContext : ISerializable
    {
        private SpiderSetting spiderSetting;        //关联的Spider配置信息
        private Url requestUrl;                     //所请求的URL实例
        private string mime;                        //MIME类型字符串,如:text/html
        private Content.ContentType contentType;    //内容类型
        private Encoding contentEncoding;           //内容编码
        private string charset;                     //内容字符集

        private NameValueCollection headers;        //HTTP响应头集合

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">请求的Url</param>
        /// <param name="responseHeaders">HTTP响应头集合</param>
        internal RequestContext(SpiderSetting setting, Url url, HttpWebResponse response)
        {
            this.spiderSetting = setting;
            this.requestUrl = url;
            this.contentType = Content.ContentType.Unknown;
            this.contentEncoding = Encoding.Default;
            this.headers = new NameValueCollection(response.Headers);

            //从Headers[contentType]字符串，如 text/html;charset=gb2312，初始化ContentType和ContentEncoding
            StringDictionary items = Utils.DetectContentTypeHeader(response.Headers[HttpResponseHeader.ContentType]);
            this.mime = items["mime"] == null ? "" : items["mime"];
            this.charset = items["charset"] == null ? "" : items["charset"];

            if (this.mime.StartsWith("text/") || this.mime == "application/x-javascript")
            {
                this.contentType = Content.ContentType.Text;
            }
            else
            {
                this.contentType = Content.ContentType.Binary;
            }

            if (this.charset == "")
            {
                this.contentEncoding = Encoding.Default;
            }
            else
            {
                this.contentEncoding = Encoding.GetEncoding(this.charset);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        private RequestContext(SerializationInfo info, StreamingContext context)
        {
            this.spiderSetting = info.GetValue("spiderSetting", typeof(SpiderSetting)) as SpiderSetting;
            this.requestUrl = info.GetValue("requestUrl", typeof(Url)) as Url;
            this.mime = info.GetString("mime");
            this.contentType = (Content.ContentType)info.GetValue("contentType", typeof(Content.ContentType));
            this.contentEncoding = info.GetValue("contentEncoding", typeof(Encoding)) as Encoding;
            this.charset = info.GetString("charset");
            this.headers = info.GetValue("headers", typeof(NameValueCollection)) as NameValueCollection;
        }

        #region properties
        /// <summary>
        /// 获取当前的Spider配置信息
        /// </summary>
        public SpiderSetting SpiderSetting
        {
            get
            {
                return spiderSetting;
            }
        }

        /// <summary>
        /// 获取当前请求的URL
        /// </summary>
        public Url RequestUrl
        {
            get
            {
                return this.requestUrl;
            }
        }

        /// <summary>
        /// 获取HTTP响应头集合
        /// </summary>
        public NameValueCollection ResponseHeaders
        {
            get
            {
                return this.headers;
            }
        }

        /// <summary>
        /// 获取Mime字符串
        /// </summary>
        public string Mime
        {
            get
            {
                return this.mime;
            }
        }

        /// <summary>
        /// 获取内容类型
        /// </summary>
        public Content.ContentType ContentType
        {
            get
            {
                return this.contentType;
            }
        }

        /// <summary>
        /// 获取Charset字符串
        /// </summary>
        public string Charset
        {
            get
            {
                return this.charset;
            }
        }

        /// <summary>
        /// 获取内容编码
        /// </summary>
        public Encoding ContentEncoding
        {
            get
            {
                return this.contentEncoding;
            }
        }

        #endregion


        /// <summary>
        /// 当前对象的字符串表示
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.requestUrl.Uri.OriginalString);
            sb.AppendLine("Response Headers:");
            for (int i = 0; i < this.headers.Count; i++)
            {
                sb.AppendFormat("{0}={1}{2}", this.headers.Keys[i], this.headers.Get(i), Environment.NewLine);
            }
            return sb.ToString();
        }

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("spiderSetting", this.spiderSetting, typeof(SpiderSetting));
            info.AddValue("requestUrl", this.requestUrl);
            info.AddValue("mime", this.mime);
            info.AddValue("contentType", this.contentType, typeof(Content.ContentType));
            info.AddValue("contentEncoding", this.contentEncoding, typeof(Encoding));
            info.AddValue("charset", this.charset);
            info.AddValue("headers", this.headers, typeof(NameValueCollection));
        }

        #endregion
    }
}
