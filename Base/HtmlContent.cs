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

using Souex.Spider.Framework.Core;
using Souex.Spider.Framework.Helpers.Html;

namespace Souex.Spider.Framework.Base
{
    /// <summary>
    /// 提供HTML内容的处理
    /// </summary>
    public class HtmlContent : ContentBase
    {
        private HtmlDocument html;
        private HtmlNode rootNode;
        private HtmlNode headNode;
        private HtmlNode bodyNode;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text"></param>
        public HtmlContent(TextContent text)
            : base(text)
        {
            this.Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (null == this.Content || !(this.Content is TextContent))
            {
                return;
            }

            this.html = new HtmlDocument();
            this.html.OptionAutoCloseOnEnd = true;
            //this.html.OptionReadEncoding = false;
            //this.html.OptionDefaultStreamEncoding = this.Content.Context.ContentEncoding;

            this.html.LoadHtml(((TextContent)this.Content).Content);

            this.rootNode = this.html.DocumentNode;
            this.headNode = this.rootNode.SelectSingleNode("//html/head");
            this.bodyNode = this.rootNode.SelectSingleNode("//html/body");
        }

        public bool Enabled
        {
            get
            {
                return null != this.html.DocumentNode;
            }
        }

        public HtmlDocument Document
        {
            get
            {
                return this.html;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (null != this.html)
            {
                this.html = null;
            }
        }
    }
}
