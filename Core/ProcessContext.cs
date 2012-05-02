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
    /// 为处理程序提供上下文信息
    /// </summary>
    [Serializable]
    public sealed class ProcessContext:System.Runtime.Serialization.ISerializable
    {
        private SpiderBase spiderBase;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal ProcessContext(SpiderBase spider)
        {
            this.spiderBase = spider;
        }

        private ProcessContext(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.spiderBase = info.GetValue("spiderBase", typeof(SpiderBase)) as SpiderBase;
        }

        /// <summary>
        /// 获取当前的SpiderBase实例
        /// </summary>
        public SpiderBase CurrentSpider
        {
            get
            {
                return this.spiderBase;
            }
        }

        #region ISerializable 成员

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("spiderBase", this.spiderBase, typeof(SpiderBase));
        }

        #endregion
    }
}
