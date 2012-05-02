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
    /// 域名后缀提供者
    /// </summary>
    [Serializable]
    public class DomainSuffixProvider : IDomainSuffixPrivoder
    {
        private static IDomainSuffixPrivoder defaultProvider;

        private string[] suffixes;

        private DomainSuffixProvider()
        {
            this.suffixes = new string[0];
        }

        private DomainSuffixProvider(string suffixString)
            :this()
        {
            if (!String.IsNullOrEmpty(suffixString))
            {
                this.suffixes = suffixString.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private DomainSuffixProvider(SerializationInfo info, StreamingContext context)
            : this(info.GetString("suffixes"))
        {
            //
        }

        /// <summary>
        /// 获取一个默认的IDomainSuffixPrivoder实例
        /// </summary>
        public static IDomainSuffixPrivoder Default
        {
            get
            {
                if (null == defaultProvider)
                {
                    defaultProvider = new DomainSuffixProvider(".com,.net,.org,.gov,.tv,.biz,.cc,.info,.cn,.com.cn,.net.cn,.org.cn,.gov.cn");
                }
                return defaultProvider;
            }
        }

        /// <summary>
        /// 依据给定的后缀，创建一个IDomainSuffixPrivoder实例
        /// </summary>
        /// <param name="suffix">域名后缀,使用逗号,分号，或者空格分隔,如：.com,.net,.org</param>
        /// <returns>IDomainSuffixPrivoder</returns>
        public static IDomainSuffixPrivoder CreateProvider(string suffix)
        {
            return new DomainSuffixProvider(suffix);
        }

        #region IDomainSuffixPrivoder 成员

        public string[] GetSuffixes()
        {
            return this.suffixes;
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("suffixes", (null == this.suffixes) ? "" : String.Join(",", this.suffixes));
        }

        #endregion
    }
}
