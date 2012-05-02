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
    /// 域名后缀提供程序接口
    /// </summary>
    public interface IDomainSuffixPrivoder:ISerializable
    {
        /// <summary>
        /// 获取所有可用的域名后缀,后缀应该以圆点开始,小写,如.com
        /// </summary>
        /// <returns>域名后缀数组</returns>
        string[] GetSuffixes();
    }
}
