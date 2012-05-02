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
    /// 表示最终URL
    /// </summary>
    [Serializable]
    public sealed class FinalUrl : Url
    {
        private Url holderUrl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <param name="relativeUrl">相对URL字符串</param>
        internal FinalUrl(string url, string relativeUrl, Url holder)
            : base(url, relativeUrl)
        {
            this.holderUrl = holder;
        }

        private FinalUrl(SerializationInfo info,StreamingContext context)
            :base(info,context)
        {
            string temp = info.GetString("holderUrl");
            if (!String.IsNullOrEmpty(temp))
            {
                this.holderUrl = Url.CreateFinalUrl(temp, null);
            }
        }

        /// <summary>
        /// 获取或设置此URL的持有URL,即此URL是从持有URL的内容中抽取的
        /// </summary>
        public Url HolderUrl
        {
            get
            {
                return this.holderUrl;
            }
            set
            {
                this.holderUrl = value;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("holderUrl", null != this.holderUrl?this.holderUrl.Uri.AbsoluteUri:"");
        }
    }
}
