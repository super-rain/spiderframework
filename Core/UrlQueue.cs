/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示URL的先进先出队列
    /// </summary>
    [Serializable]
    public class UrlQueue : QueueBase<Url>
    {
        internal UrlQueue()
            : base()
        {
            //
        }
    }
}
