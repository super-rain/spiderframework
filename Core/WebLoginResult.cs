using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示WEB登录的返回结果，无法从外部实例化此类
    /// </summary>
    public sealed class WebLoginResult
    {
        private CookieCollection cookies;
        private WebHeaderCollection responseHeaders;
        private string content;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="response">HttpWebResponse</param>
        /// <param name="responseEncoding">Encoding</param>
        internal WebLoginResult(HttpWebResponse response, Encoding responseEncoding)
        {
            if (null != response.Headers)
            {
                this.responseHeaders = new WebHeaderCollection();
                for (int i = 0; i < response.Headers.Count; i++)//Copy Headers
                {
                    this.responseHeaders.Add(response.Headers.Keys[i], response.Headers[i]);
                }
            }

            if (null != response.Cookies)
            {
                this.cookies = new CookieCollection();
                Cookie[] cks = new Cookie[response.Cookies.Count];
                response.Cookies.CopyTo(cks, 0);
                foreach (Cookie ck in cks)
                {
                    this.cookies.Add(ck);
                }
            }

            StringDictionary items = Utils.DetectContentTypeHeader(response.Headers[HttpResponseHeader.ContentType]);
            string charset = items["charset"] == null ? "" : items["charset"];
            Encoding enc = Encoding.Default;
            if (charset == "")
            {
                enc = null == responseEncoding ? Encoding.Default : responseEncoding;
            }
            else
            {
                enc = Encoding.GetEncoding(charset);
            }

            using (Stream stream = response.GetResponseStream())
            {
                StringBuilder text = new StringBuilder();
                byte[] buffer = new byte[1024];
                int n = 0;
                while ((n = stream.Read(buffer, 0, 1024)) > 0)
                {
                    text.Append(enc.GetString(buffer, 0, n));
                }
                this.content = text.ToString();
            }
        }

        /// <summary>
        /// 获取响应头包含的Cookie信息
        /// </summary>
        public CookieCollection Cookies
        {
            get
            {
                return this.cookies;
            }
        }

        /// <summary>
        /// 获取响应头集合
        /// </summary>
        public WebHeaderCollection ResponseHeaders
        {
            get
            {
                return this.responseHeaders;
            }
        }

        /// <summary>
        /// 获取响应内容
        /// </summary>
        public string Text
        {
            get
            {
                return this.content;
            }
        }
    }
}
