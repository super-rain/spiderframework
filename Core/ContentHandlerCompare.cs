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
    /// 实现两个处理程序的优先级比较
    /// </summary>
    internal class ContentHandlerPriorityCompare:IComparer<IContentHandler>
    {
        #region IComparer<IContentHandler> 成员

        public int Compare(IContentHandler x, IContentHandler y)
        {
            return x.Priority - y.Priority;
        }

        #endregion
    }
}
