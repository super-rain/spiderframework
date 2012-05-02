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
    /// 表示等待进一步处理的内容队列
    /// </summary>
    [Serializable]
    public class ContentQueue:QueueBase<Content>
    {
        internal ContentQueue()
            :base()
        {
        }
    }
}
