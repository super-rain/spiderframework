/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示文本内容
    /// </summary>
    [Serializable]
    public class TextContent:Content
    {
        private string content;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">请求的上下文</param>
        internal TextContent(RequestContext context)
            : base(context)
        {
            this.content = "";
        }

        /// <summary>
        /// 从给定的流初始化文本内容,并立即销毁给第的流对象
        /// </summary>
        /// <param name="stream">Stream</param>
        protected sealed override void ReadStream(Stream stream, out long bytes)
        {
            bytes = InValidContentLength;
            StringBuilder text = new StringBuilder();
            byte[] buffer = new byte[this.Context.SpiderSetting.ReadBufferSize];
            int n = 0;
            while ((n = stream.Read(buffer, 0, this.Context.SpiderSetting.ReadBufferSize)) > 0)
            {
                text.Append(this.Context.ContentEncoding.GetString(buffer,0,n));
                bytes += n;
            }
            this.content = text.ToString();
        }

        private TextContent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.content = info.GetString("content");
        }

        /// <summary>
        /// 获取文本内容的字符串表示
        /// </summary>
        public string Content
        {
            get
            {
                return this.content;
            }
        }


        /// <summary>
        /// ToString,返回文本内容
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.content;
        }

        /// <summary>
        /// 将当前文本内容保存到文件,编码与响应内容的编码一致
        /// </summary>
        /// <param name="file">string</param>
        /// <returns>string</returns>
        public override string SaveToFile(string file)
        {
            File.WriteAllText(file, this.content, this.Context.ContentEncoding);
            return file;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.content = null;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("content", this.content);
        }
    }
}
