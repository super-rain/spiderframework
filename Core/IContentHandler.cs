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
    /// 内容处理程序接口,采集到的内容会提交到此接口的一个或多个实例
    /// </summary>
    public interface IContentHandler:ISerializable
    {
        /// <summary>
        /// 根据上下文参数,处理给定的内容
        /// </summary>
        /// <param name="content">Content实例</param>
        /// <param name="context">上下文参数</param>
        void Process(Content content,ProcessContext context);

        /// <summary>
        /// 处理程序优先级,越大执行顺序越靠前
        /// </summary>
        int Priority { get; set; }
    }
}
