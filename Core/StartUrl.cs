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
    /// 表示一个起始URL
    /// </summary>
    [Serializable]
    public sealed class StartUrl : Url
    {
        internal StartUrl()
            : base("", "")
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">起始URL的字符串表示</param>
        public StartUrl(string url)
            : base(url, "")
        {
            //
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">绝对起始URL的字符串表示</param>
        /// <param name="relativeUrl">相对URL的字符串表示</param>
        public StartUrl(string url, string relativeUrl)
            : base(url, relativeUrl)
        {
            //
        }


        private StartUrl(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            //
        }
    }
}
