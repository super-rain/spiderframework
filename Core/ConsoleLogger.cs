using System;
using System.Collections.Generic;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示控制台日志的实现
    /// </summary>
    [Serializable]
    public class ConsoleLogger : ILogger
    {
        private static ConsoleLogger instance;
        private static object locker = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        internal ConsoleLogger()
            : base()
        {
        }

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns>ConsoleLogger</returns>
        public static ILogger GetInstance()
        {
            lock (locker)
            {
                if (null == instance)
                {
                    lock (locker)
                    {
                        instance = new ConsoleLogger();
                    }
                }
                return instance;
            }
        }

        #region ILogger 成员

        /// <summary>
        /// 在控制台环境中打印日志消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public virtual void WriteLog(object message)
        {
            Console.WriteLine(message);
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            //
        }

        #endregion
    }
}
