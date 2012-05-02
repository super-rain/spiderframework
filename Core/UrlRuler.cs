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
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示一个URL规则
    /// </summary>
    public abstract class UrlRuler:ISerializable
    {
        private string httpMethod;
        private UrlTypes urlType;
        private NameValueCollection appendParams;
        private NameValueCollection ignoreParams;
        private ICollection<IContentHandler> contentHandlers;

        protected UrlRuler()
        {
            this.httpMethod = "GET";
            this.urlType = 0x0;
            this.appendParams = new NameValueCollection();
            this.ignoreParams = new NameValueCollection();
            this.contentHandlers = new List<IContentHandler>();
        }

        protected UrlRuler(SerializationInfo info, StreamingContext context)
        {
            this.httpMethod = info.GetString("httpMethod");
            this.urlType = (UrlTypes)info.GetValue("urlType", typeof(UrlTypes));
            this.appendParams = info.GetValue("appendParams", typeof(NameValueCollection)) as NameValueCollection;
            this.ignoreParams = info.GetValue("ignoreParams", typeof(NameValueCollection)) as NameValueCollection;
            this.contentHandlers = info.GetValue("contentHandlers", typeof(ICollection<IContentHandler>)) as ICollection<IContentHandler>;
        }

        /// <summary>
        /// 获取或设置当前规则所规定HTTP访问方式,GET或POST
        /// </summary>
        public string HttpMethod
        {
            get
            {
                return this.httpMethod;
            }
            set
            {
                this.httpMethod = value;
            }
        }

        /// <summary>
        /// 获取或设置当前规则所定义的URL类型
        /// </summary>
        public UrlTypes UrlType
        {
            get
            {
                return this.urlType;
            }
            set
            {
                this.urlType = value;
            }
        }

        /// <summary>
        /// 获取或设置当前规则所定义的内容处理程序接口集合
        /// </summary>
        public ICollection<IContentHandler> ContentHandlers
        {
            get
            {
                return this.contentHandlers;
            }
        }

        /// <summary>
        /// 获取当前规则所定义的附加参数
        /// </summary>
        public NameValueCollection AppendParmas
        {
            get
            {
                return this.appendParams;
            }
        }

        /// <summary>
        /// 获取当前规则所定义的附加参数
        /// </summary>
        public NameValueCollection IgnoreParmas
        {
            get
            {
                return this.ignoreParams;
            }
        }

        /// <summary>
        /// 检查是否匹配给定的URL
        /// </summary>
        /// <param name="url">string</param>
        /// <returns>bool</returns>
        public abstract bool IsMatch(string url);

        /// <summary>
        /// 创建一个正则表达式类型的URL规则
        /// </summary>
        /// <param name="regExpress">有效的正则表达式</param>
        /// <param name="ignoreCase">指示是否忽略大小写</param>
        /// <returns>UrlRuler</returns>
        public static UrlRuler CreateRegexRuler(string regExpress, UrlTypes type, bool ignoreCase)
        {
            return new RegexUrlRuler(new Regex(regExpress, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None), type);
        }

        /// <summary>
        /// 创建一个大小写敏感的正则表达式类型的URL规则
        /// </summary>
        /// <param name="regExpress">有效的正则表达式</param>
        /// <returns>UrlRuler</returns>
        public static UrlRuler CreateRegexRuler(string regExpress, UrlTypes type)
        {
            return CreateRegexRuler(regExpress,type, false);
        }

        /// <summary>
        /// 根据自定义的Regex实例,创建一个正则表达式类型的URL规则
        /// </summary>
        /// <param name="regex">Regex实例</param>
        /// <returns>UrlRuler</returns>
        public static UrlRuler CreateRegexRuler(Regex regex, UrlTypes type)
        {
            return new RegexUrlRuler(regex, type);
        }

        /// <summary>
        /// 创建一个范围类型的URL规则
        /// </summary>
        /// <param name="rangeModel">IUrlRangeModel的实例</param>
        /// <param name="strictMatch">指示是否采用严格匹配</param>
        /// <returns>UrlRuler</returns>
        public static UrlRuler CreateRangeRuler(IUrlRangeModel rangeModel,UrlTypes type, bool strictMatch)
        {
            return new RangeUrlRuler(rangeModel,type, strictMatch);
        }

        /// <summary>
        /// 创建一个严格类型的URL规则
        /// </summary>
        /// <param name="urls">URL字符串数组</param>
        /// <param name="ignoreCase">指示是否忽略大小写</param>
        /// <returns>UrlRuler</returns>
        public static UrlRuler CreateStrictRuler(string[] urls,bool ignoreCase)
        {
            return new StrictUrlRuler(urls, ignoreCase);
        }

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("httpMethod", this.httpMethod);
            info.AddValue("urlType", this.urlType, typeof(UrlTypes));
            info.AddValue("appendParams", this.appendParams, typeof(NameValueCollection));
            info.AddValue("ignoreParams", this.ignoreParams, typeof(NameValueCollection));
            info.AddValue("contentHandlers", this.contentHandlers, typeof(ICollection<IContentHandler>));
        }

        #endregion
    }
}
