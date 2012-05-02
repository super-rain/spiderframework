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
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示采集到的内容
    /// </summary>
    [Serializable]
    public abstract class Content : IDisposable,ISerializable
    {
        protected const long InValidContentLength = -1L;

        /// <summary>
        /// 内容类型枚举值
        /// </summary>
        [Serializable]
        public enum ContentType
        {
            Unknown = 0,
            Text = 1,
            Binary = 2
        }

        private Url rawUrl;             //当前内容的原始URL
        private RequestContext context; //与当前内容关联的请求上下文信息
        private long contentLength;     //字节长度

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">请求上下文</param>
        protected Content(RequestContext context)
        {
            this.context = context;
            this.rawUrl = this.context.RequestUrl;
            this.contentLength = InValidContentLength;
        }

        protected Content(SerializationInfo info, StreamingContext context)
        {
            this.rawUrl = info.GetValue("rawUrl", typeof(Url)) as Url;
            this.context = info.GetValue("context", typeof(RequestContext)) as RequestContext;
            this.contentLength = info.GetInt64("contentLength");
        }

        #region properties
        /// <summary>
        /// 获取当前的URL
        /// </summary>
        public Url RawUrl
        {
            get
            {
                return this.rawUrl;
            }
        }

        /// <summary>
        /// 获取当前的上下文信息
        /// </summary>
        public RequestContext Context
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>
        /// 获取内容的字节长度
        /// </summary>
        public long ContentLength
        {
            get
            {
                return this.contentLength;
            }
        }
        #endregion

        /// <summary>
        /// 依据上下文参数创建Content实例，内容类型无效时返回NULL
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>Content</returns>
        public static Content Create(RequestContext context)
        {
            switch (context.ContentType)
            {
                case ContentType.Text:
                    return new TextContent(context);
                case ContentType.Binary:
                    return new BinaryContent(context);
            }
            return null;
        }

        /// <summary>
        /// 在派生类中重写此方法，用以读取流内容
        /// </summary>
        /// <param name="stream">Stream</param>
        internal void Read(Stream stream)
        {
            this.ReadStream(stream, out this.contentLength);
        }

        /// <summary>
        /// 在派生类中重写此方法，用以读取流内容
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>读取到的字节数</returns>
        protected abstract void ReadStream(Stream stream,out long bytes);

        /// <summary>
        /// 在派生类中重写此方法,用以保存当前内容到指定文件,成功则返回指定文件名,否则返回String.Empty.在调用此方法之前,应该调用Read()方法初始化流内容
        /// </summary>
        /// <param name="file">指定文件</param>
        /// <returns>string</returns>
        public virtual string SaveToFile(string file)
        {
            return String.Empty;
        }

        /// <summary>
        /// 通过比较两个内容的RawUrl是否相同来判断是否相等
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>bool</returns>
        public new bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Content))
            {
                return false;
            }
            return this.rawUrl.CheckSum == ((Content)obj).rawUrl.CheckSum;
        }

        #region IDisposable 成员

        public virtual void Dispose()
        {
            this.context = null;
        }

        #endregion

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("rawUrl", this.rawUrl, typeof(Url));
            info.AddValue("context", this.context, typeof(RequestContext));
            info.AddValue("contentLength", this.contentLength);
        }

        #endregion
    }
}
