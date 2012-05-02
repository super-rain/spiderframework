/* 
 * SpiderLib,基于规则的采集程序基础框架
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
    /// 表示一个链接抽取器,从HTML内容中抽取链接URL
    /// </summary>
    [Serializable]
    public class HtmlUrlExtractor : UrlExtractor
    {
        private bool tagA;          //指示是否开启A标签链(href)接抽取
        private bool tagImg;        //指示是否开启IMG标签链(src)接抽取
        private string imgTypes;    //指示要抽取的图片类型

        private Regex regexA;       //A标签正则
        private Regex regexImg;     //IMG标签正则

        /// <summary>
        /// 构造函数
        /// </summary>
        public HtmlUrlExtractor()
            : base()
        {
            this.tagA = true;
            this.tagImg = false;
            this.imgTypes = "gif|jpg|jpeg|png";
            this.SetRegEx();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="onTagA">指示是否开启A标签</param>
        /// <param name="onTagImg">指示是否开启IMG标签</param>
        public HtmlUrlExtractor(bool onTagA, bool onTagImg)
            : this()
        {
            this.tagA = onTagA;
            this.tagImg = onTagImg;

            this.SetRegEx();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="onTagA">指示是否开启A标签</param>
        /// <param name="onTagImg">指示是否开启IMG标签</param>
        /// <param name="imgExtensions">开启IMG标签时,指定图片扩展名,格式如:gif|jpg|png.</param>
        public HtmlUrlExtractor(bool onTagA, bool onTagImg, string imgExtensions)
            : this(onTagA, onTagImg)
        {
            this.imgTypes = imgExtensions;
            this.SetRegEx();
        }

        private HtmlUrlExtractor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.TagA = info.GetBoolean("tagA");
            this.TagImg = info.GetBoolean("tagImg");
            this.imgTypes = info.GetString("imgTypes");
        }


        /// <summary>
        /// 获取或设置标签A开关,为true表示提取A标签的href属性
        /// </summary>
        public bool TagA
        {
            get
            {
                return this.tagA;
            }
            set
            {
                this.tagA = value;
                this.SetRegEx();
            }
        }

        /// <summary>
        /// 获取或设置标签IMG开关,为true表示提取IMG标签的src属性
        /// </summary>
        public bool TagImg
        {
            get
            {
                return this.tagImg;
            }
            set
            {
                this.tagImg = value;
                this.SetRegEx();
            }
        }

        /// <summary>
        /// 获取或设置抽取的图片类型,形式如:gif|jpg|png
        /// </summary>
        public string ImgTypes
        {
            get
            {
                return this.imgTypes;
            }
            set
            {
                this.imgTypes = value;
            }
        }

        /// <summary>
        /// 设置正则表达式
        /// </summary>
        private void SetRegEx()
        {
            if (this.tagA)
            {
                this.regexA = UrlExtractor.AnchorHrefRegex;
            }
            else
            {
                this.regexA = null;
            }

            if (this.tagImg)
            {
                this.regexImg = new Regex(String.Format("<\\s*img[^>]+src=('|\\\")?\\s*(?<SRC>[^>\\s]+{0})\\1\\s*[^>]*>", this.ConvertImgTypePattern()), RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            else
            {
                this.regexImg = null;
            }
        }

        /// <summary>
        /// 将图片类型的字符串定义转化为正则表达式
        /// </summary>
        /// <returns></returns>
        private string ConvertImgTypePattern()
        {
            if (String.IsNullOrEmpty(this.imgTypes))
            {
                return "";
            }

            string s = "";
            if (this.imgTypes.Split('|').Length < 2)
            {
                s = this.imgTypes.Trim();
            }
            else
            {
                s = String.Join("|", this.imgTypes.Split('|'));
            }

            return s == "" ? "" : "\\.(" + s + ")";
        }

        /// <summary>
        /// 从给定内容中提取URL字符串,返回一个IEnumerable泛型集合,元素为string
        /// </summary>
        /// <param name="content">Content实例</param>
        /// <returns>IEnumerable</returns>
        protected override IEnumerable<KeyValuePair<string, string>> FindUrls(Content content)
        {
            if (null == content || content.GetType() != typeof(TextContent))
            {
                return null;
            }

            List<KeyValuePair<string, string>> urls = new List<KeyValuePair<string, string>>();
            string url, text;
            if (null != this.regexA)
            {
                foreach (Match m in this.regexA.Matches(((TextContent)content).Content))
                {
                    url = m.Groups["URL"].Value;
                    text = m.Groups["TEXT"].Value;
                    if (null != url && "" != url)
                    {
                        urls.Add(new KeyValuePair<string, string>(url, text));
                    }
                }
            }

            if (null != this.regexImg)
            {
                foreach (Match m in this.regexImg.Matches(((TextContent)content).Content))
                {
                    url = m.Groups["SRC"].Value;
                    text = "";
                    Match match = UrlExtractor.ImageAltRegex.Match(m.Value);
                    if (match.Success)
                    {
                        text = match.Groups["ALT"].Value;
                    }
                    if (null != url && "" != url)
                    {
                        urls.Add(new KeyValuePair<string, string>(url, text));
                    }
                }
            }
            return urls;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("tagA", this.tagA);
            info.AddValue("tagImg", this.tagImg);
            info.AddValue("imgTypes", this.imgTypes);
        }
    }
}
