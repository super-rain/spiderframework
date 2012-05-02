/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示一个基于正则表达式的URL规则
    /// </summary>
    [Serializable]
    public class RegexUrlRuler : UrlRuler
    {
        private Regex regex;

        /// <summary>
        /// 构造函数
        /// </summary>
        private RegexUrlRuler()
            : base()
        {
            //
        }

        protected RegexUrlRuler(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.regex = info.GetValue("regex", typeof(Regex)) as Regex;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="regObj">Regex实例</param>
        internal RegexUrlRuler(Regex regObj, UrlTypes urlType)
            : this()
        {
            this.regex = regObj;
            this.UrlType = urlType;
        }

        /// <summary>
        /// 检验给定的URL是否符合当前规则
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <returns></returns>
        public override bool IsMatch(string url)
        {
            if (null == this.regex || String.IsNullOrEmpty(url))
            {
                return false;
            }
            bool bln = this.regex.IsMatch(url);
            return bln;
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("regex", this.regex, typeof(Regex));
        }
    }
}
