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
    /// 表示一个严格匹配模式的URL规则,用于比较两个URL是否相同
    /// </summary>
    public class StrictUrlRuler : UrlRuler
    {
        private string[] urls;
        private bool ignoreCase;

        /// <summary>
        /// 构造函数
        /// </summary>
        private StrictUrlRuler()
            : base()
        {
            this.urls = new string[0];
            this.ignoreCase = false;
        }

        /// <summary>
        /// 构造函数,默认大小写敏感
        /// </summary>
        /// <param name="urlString">URL数组</param>
        internal StrictUrlRuler(string[] urlString)
            : this()
        {
            this.urls = urlString;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="urlString">URL数组</param>
        /// <param name="igCase">指示是否忽略大小写</param>
        internal StrictUrlRuler(string[] urlString, bool igCase)
            : this(urlString)
        {
            this.urls = urlString;
            this.ignoreCase = igCase;
        }

        /// <summary>
        /// 检验给定的URL是否与实例中的URL数组内的至少一项相同
        /// </summary>
        /// <param name="url">给定的URL</param>
        /// <returns>存在相同项是返回ture,否则返回false</returns>
        public override bool IsMatch(string url)
        {
            if (null == url || url == "")
            {
                return false;
            }

            if (this.ignoreCase)
            {
                url = url.ToLower();
                for(int i=0;i<this.urls.Length;i++)
                {
                    this.urls[i] = this.urls[i].ToLower();
                }
            }

            return Array.IndexOf(this.urls, url) > -1;
        }
    }

}
