using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// WEB登录接口
    /// </summary>
    public interface IWebLogin
    {
        /// <summary>
        /// 尝试向登录程序提交用户名和密码信息，获取返回结果
        /// </summary>
        /// <param name="loginUrl">数据提交的目标URL</param>
        /// <param name="method">提交方法</param>
        /// <param name="requestEncoding">请求编码</param>
        /// <param name="responseEncoding">响应编码</param>
        /// <param name="args">随请求一起发送的数据</param>
        /// <returns>WebLoginResult</returns>
        WebLoginResult Login(string loginUrl, string method, Encoding requestEncoding, Encoding responseEncoding, NameValueCollection args);
    }

}
