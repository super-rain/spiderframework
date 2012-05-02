/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示一个范围规则
    /// </summary>
    [Serializable]
    public class RangeUrlRuler : UrlRuler
    {
        private IUrlRangeModel rangModel;
        private bool strictMatch;

        private RangeUrlRuler()
            : base()
        {
            this.strictMatch = true;
        }

        protected RangeUrlRuler(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.rangModel = info.GetValue("rangModel", typeof(IUrlRangeModel)) as IUrlRangeModel;
            this.strictMatch = info.GetBoolean("strictMatch");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="model">规则使用的模型</param>
        /// <param name="urlType">URL类型</param>
        /// <param name="strict">是否进行严格匹配</param>
        internal RangeUrlRuler(IUrlRangeModel model, UrlTypes urlType, bool strict)
            : this()
        {
            this.rangModel = model;
            this.UrlType = urlType;
            this.strictMatch = strict;
        }

        /// <summary>
        /// 获取或设置当前范围规则使用的模型
        /// </summary>
        public IUrlRangeModel RangModel
        {
            get
            {
                return this.rangModel;
            }
            set
            {
                this.rangModel = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值,指示是否进行严格匹配,即比较两个字符串是否相同.
        /// 否则,仅检查两个字符串是否存在包含关系(检查结尾).
        /// 在非严格方式下,对于两个字符串A和B,如果存在A以B结尾,或者B以A结尾,则认为是匹配的
        /// </summary>
        public bool StrictMatch
        {
            get
            {
                return this.strictMatch;
            }
            set
            {
                this.strictMatch = value;
            }
        }

        /// <summary>
        /// 判断是否匹配给定的url
        /// </summary>
        /// <param name="url">string</param>
        /// <returns>bool</returns>
        public override bool IsMatch(string url)
        {
            if (null == this.rangModel || String.IsNullOrEmpty(url))
            {
                return false;
            }

            string[] list = this.rangModel.GenerateList();

            if (null == list || list.Length < 1)
            {
                return false;
            }

            foreach (string s in list)
            {
                if ((this.strictMatch && s == url) ||
                    //在非严格方式下,对于两个字符串A和B,如果存在A以B结尾,或者B以A结尾,或者A以B开头,或者B以A开头,则认为是匹配的
                    (!this.strictMatch &&
                    (url.StartsWith(s, StringComparison.InvariantCultureIgnoreCase) || s.StartsWith(url, StringComparison.InvariantCultureIgnoreCase) ||
                    url.EndsWith(s, StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(url, StringComparison.InvariantCultureIgnoreCase))))
                {
                    return true;
                }
            }
            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("rangModel", this.rangModel, typeof(IUrlRangeModel));
            info.AddValue("strictMatch", this.strictMatch);
        }
    }
}
