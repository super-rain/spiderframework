using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// WEB登录辅助类
    /// </summary>
    public class WebLoginBase : IWebLogin
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WebLoginBase()
        {
            //
        }

        #region IWebLogin 成员

        /// <summary>
        /// 尝试向登录程序提交用户名和密码信息，获取返回结果
        /// </summary>
        /// <param name="loginUrl">数据提交的目标URL</param>
        /// <param name="method">提交方法</param>
        /// <param name="requestEncoding">请求编码</param>
        /// <param name="responseEncoding">响应编码</param>
        /// <param name="args">随请求一起发送的数据</param>
        /// <returns>WebLoginResult，失败时返回NULL</returns>
        public WebLoginResult Login(string loginUrl, string method, Encoding requestEncoding, Encoding responseEncoding, NameValueCollection args)
        {
            HttpWebRequest request = WebRequest.Create(loginUrl) as HttpWebRequest;

            string agent = this.GetUserAgent();
            request.UserAgent = (null == agent || "" == agent) ? SpiderSetting.DefaultUserAgent : agent;

            //带参数的POST请求
            if (method == "POST" && null != args)
            {
                request.Method = "POST";
                request.ContentType = String.Format("application/x-www-form-urlencoded;charset={0}", requestEncoding.WebName);
                byte[] data = SpiderBase.GetRequestData(args, null == requestEncoding ? Encoding.Default : requestEncoding);
                request.ContentLength = data.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                }
            }

            IWebProxy proxy = this.GetWebProxy();
            if (proxy != null)
            {
                request.Proxy = proxy;
            }

            string referer = this.GetReferer();
            if (null != referer && "" != referer)
            {
                request.Referer = referer;
            }

            HttpWebResponse response = null;
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

            return new WebLoginResult(response, responseEncoding);
        }

        #endregion

        /// <summary>
        /// 以默认POST方法向登录程序提交用户名和密码信息，获取返回结果
        /// </summary>
        /// <param name="loginUrl">数据提交的目标URL</param>
        /// <param name="method">提交方法</param>
        /// <param name="requestEncoding">请求编码</param>
        /// <param name="responseEncoding">响应编码</param>
        /// <param name="args">随请求一起发送的数据</param>
        /// <returns>WebLoginResult，失败时返回NULL</returns>
        public WebLoginResult Login(string loginUrl, Encoding requestEncoding, Encoding responseEncoding, NameValueCollection args)
        {
            return Login(loginUrl, "POST", requestEncoding, responseEncoding, args);
        }

        /// <summary>
        /// 以默认POST方法，Encoding.Default编码，提交登录数据
        /// </summary>
        /// <param name="loginUrl">数据提交的目标URL</param>
        /// <param name="args">随请求一起发送的数据</param>
        /// <returns>WebLoginResult，失败时返回NULL</returns>
        public WebLoginResult Login(string loginUrl, NameValueCollection args)
        {
            return Login(loginUrl, "POST", Encoding.Default, Encoding.Default, args);
        }

        /// <summary>
        /// 在派生类中重写此方法，返回一个IWebProxy实例，以便的请求发送之前设定网络代理,返回NULL将不使用代理
        /// </summary>
        /// <returns>IWebProxy</returns>
        protected virtual IWebProxy GetWebProxy()
        {
            return null;
        }

        /// <summary>
        /// 在派生类中重写此方法，返回一个Url字符串，以便的请求发送之前设定URLREFERER，返回NULL或空表示忽略此请求头
        /// </summary>
        /// <returns>string</returns>
        protected virtual string GetReferer()
        {
            return null;
        }

        /// <summary>
        /// 在派生类中重写此方法，返回一个字符串，以便的请求发送之前设定User-Agent信息，返回NULL或空将使用内置的User-Agent请求头
        /// </summary>
        /// <returns></returns>
        protected virtual string GetUserAgent()
        {
            return null;
        }


    }
}
