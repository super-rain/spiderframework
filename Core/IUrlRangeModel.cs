/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示范围规则的模型
    /// </summary>
    public interface IUrlRangeModel:ISerializable
    {
        /// <summary>
        /// 使用的URL模板
        /// </summary>
        string UrlTemplate { get; set; }

        /// <summary>
        /// 替换标记字符
        /// </summary>
        string ReplaceMark { get; set; }

        /// <summary>
        /// 依据规则,替换URL模板中的标记字符,生成Url列表
        /// </summary>
        /// <returns></returns>
        string[] GenerateList();
    }
}
