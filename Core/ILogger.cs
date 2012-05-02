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
    /// 日志接口
    /// </summary>
    public interface ILogger:ISerializable
    {
        /// <summary>
        /// 写入日志消息
        /// </summary>
        /// <param name="message">消息内容</param>
        void WriteLog(object message);
    }
}
