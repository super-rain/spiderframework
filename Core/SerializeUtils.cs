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
using System.Runtime.Serialization.Formatters.Binary;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 序列化辅助类
    /// </summary>
    public static class SerializeUtils
    {
        #region  SpiderSetting
        /// <summary>
        /// 从给定的流,反序列化SpiderSetting实例,失败时返回NULL
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>SpiderSetting</returns>
        public static SpiderSetting Deserialize(Stream stream)
        {
            object obj = DeserializeObject(stream);
            if (null != obj && obj is SpiderSetting)
            {
                return obj as SpiderSetting;
            }
            return null;
        }

        /// <summary>
        /// 从给定的文件,反序列化SpiderSetting实例,失败时返回NULL
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns>SpiderSetting</returns>
        public static SpiderSetting Deserialize(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }

            using (Stream stream = File.OpenRead(file))
            {
                return Deserialize(stream);
            }
        }
        #endregion



        #region DeserializeObject
        private static object DeserializeObject(Stream stream)
        {
            if (null == stream)
            {
                return null;
            }

            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (stream)
                {
                    object obj = formatter.Deserialize(stream);
                    return obj;
                }
            }
            catch
            {
                //
            }

            return null;
        }
        #endregion
    }
}
