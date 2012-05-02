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
    /// 表示索引URL
    /// </summary>
    [Serializable]
    public sealed class IndexUrl : Url
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">基URL字符串</param>
        /// <param name="relativeUrl">相对URL字符串</param>
        internal IndexUrl(string url, string relativeUrl)
            : base(url, relativeUrl)
        {
            //
        }

        private IndexUrl(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            //
        }
    }
}
