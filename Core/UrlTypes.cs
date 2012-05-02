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
    /// 表示URL类型的枚举
    /// </summary>
    [Serializable]
    public enum UrlTypes : byte
    {
        /// <summary>
        /// 索引URL
        /// </summary>
        Index = 0x1,

        /// <summary>
        /// 最终URL
        /// </summary>
        Final = 0x2
    }
}
