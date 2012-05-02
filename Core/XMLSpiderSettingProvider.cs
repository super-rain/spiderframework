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

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示基于文件的爬虫配置提供程序
    /// </summary>
    public sealed class XMLSpiderSettingProvider:ISpiderSettingProvider,IDisposable
    {
        private string fileName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="file"></param>
        public XMLSpiderSettingProvider(string file)
        {
            this.fileName = file;
        }

        #region ISpiderSettingProvider 成员

        /// <summary>
        /// 读取配置文件,初始化SpiderSetting实例
        /// </summary>
        /// <param name="setting">SpiderSetting实例</param>
        public void ReadSetting(SpiderSetting setting)
        {
            if (Directory.Exists(this.fileName))
            {
                throw new FileNotFoundException(this.fileName + " not be found!");
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(this.fileName);
        }

        /// <summary>
        /// 写入配置信息到文件
        /// </summary>
        /// <param name="setting"></param>
        public void WriteSetting(SpiderSetting setting)
        {

        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            //
        }

        #endregion
    }
}