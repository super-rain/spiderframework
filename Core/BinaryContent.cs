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
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示二进制内容
    /// </summary>
    [Serializable]
    public class BinaryContent : Content
    {
        /// <summary>
        /// 二进制流
        /// </summary>
        private Stream binaryStream;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">请求上下文</param>
        internal BinaryContent(RequestContext context)
            : base(context)
        {
            //
        }

        protected BinaryContent(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {
            this.binaryStream = info.GetValue("binaryStream", typeof(Stream)) as Stream;
        }

        /// <summary>
        /// 读取流内容
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>读取到的字节数</returns>
        protected override void ReadStream(Stream stream,out long bytes)
        {
            bytes = InValidContentLength;
            if (!stream.CanRead)
            {
                return;
            }

            //if (stream.Length <= this.Context.SpiderSetting.MemLimitSize)
            {
                this.binaryStream = new MemoryStream();
            }
            //else
            //{
            //    string file = this.Context.SpiderSetting.DepositePath + this.Context.RequestUrl.CheckSum;
            //    this.binaryStream = File.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            //}

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            byte[] buffer = new byte[this.Context.SpiderSetting.ReadBufferSize];
            int n=0;
            while ((n=stream.Read(buffer, 0, this.Context.SpiderSetting.ReadBufferSize)) > 0)
            {
                this.binaryStream.Write(buffer, 0, n);
                bytes += n;
            }
        }

        /// <summary>
        /// 获取当前内容的流
        /// </summary>
        public Stream BinaryStream
        {
            get
            {
                return this.binaryStream;
            }
        }

        /// <summary>
        /// 存储当前二进制内容到指定的文件,在调用此方法之前,应该调用Read()方法初始化流内容
        /// </summary>
        /// <param name="file">目标文件</param>
        /// <returns>string</returns>
        public override string SaveToFile(string file)
        {
            if (null == this.binaryStream)
            {
                return String.Empty;
            }

            //如果是文件流,直接Copy至目标文件
            if (this.binaryStream.GetType() == typeof(FileStream))
            {
                string fileName = this.Context.SpiderSetting.DepositePath + this.Context.RequestUrl.CheckSum;
                File.Copy(fileName, file);
                return file;
            }

            using (FileStream fs = File.OpenWrite(file))
            {
                this.binaryStream.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[this.Context.SpiderSetting.ReadBufferSize];
                int n = 0;
                while ((n = this.binaryStream.Read(buffer, 0, this.Context.SpiderSetting.ReadBufferSize)) > 0)
                {
                    fs.Write(buffer, 0, n);
                }
            }

            return file;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public sealed override void Dispose()
        {
            base.Dispose();
            if (null != this.binaryStream)
            {
                this.binaryStream.Dispose();
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("binaryStream", this.binaryStream, typeof(Stream));
        }
    }
}
