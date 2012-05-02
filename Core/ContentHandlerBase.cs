using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    public abstract class ContentHandlerBase:IContentHandler
    {
        private int priority;

        /// <summary>
        /// 反序列化构造函数
        /// </summary>
        /// <param name="info">System.Runtime.Serialization.SerializationInfo</param>
        /// <param name="context">System.Runtime.Serialization.StreamingContext</param>
        protected ContentHandlerBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            try
            {
                this.priority = info.GetInt32("priority");
            }
            catch
            {
                //
            }
        }

        #region IContentHandler 成员

        /// <summary>
        /// 在派生类中重写此方法，对内容进行处理
        /// </summary>
        /// <param name="content">Content实例</param>
        /// <param name="context">ProcessContext上下文信息</param>
        public abstract void Process(Content content, ProcessContext context);

        /// <summary>
        /// 获取或设置当前处理程序的优先级
        /// </summary>
        public int Priority
        {
            get
            {
                return this.priority;
            }
            set
            {
                this.priority = value;
            }
        }

        #endregion

        #region ISerializable 成员

        /// <summary>
        /// 在派生类中重写此方法，对序列化进行支持
        /// </summary>
        /// <param name="info">System.Runtime.Serialization.SerializationInfo</param>
        /// <param name="context">System.Runtime.Serialization.StreamingContext</param>
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("priority", this.priority);
        }

        #endregion
    }
}
