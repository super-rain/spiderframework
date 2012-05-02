/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// URL求和算法接口
    /// </summary>
    public interface IUrlCheckSum:ISerializable
    {
        /// <summary>
        /// 计算给定URL的校验和
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>uint</returns>
        UInt32 CheckSum(Url url);

        /// <summary>
        /// 获取或设置要忽略的参数集合,设置{key=KeyName,value=*}将忽略所有的KeyName,允许为空集合或NULL
        /// </summary>
        NameValueCollection IgnoreParmasCollection { get; set; }
    }
}
