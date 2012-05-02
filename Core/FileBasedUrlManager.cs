/* 
 * SpiderLib,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示一个基于文件存储的URL管理器
    /// </summary>
    public sealed class FileBasedUrlManager:UrlManager
    {
        private string filename;
        /// <summary>
        /// 构造函数
        /// </summary>
        public FileBasedUrlManager(string file)
            :base()
        {
            this.filename = file;
        }

        /// <summary>
        /// 读取历史URL
        /// </summary>
        /// <returns></returns>
        protected override IList<Url> ReadHistory()
        {
            return null;
        }

        /// <summary>
        /// 保存历史URL
        /// </summary>
        /// <param name="dict"></param>
        protected override void WriteHistory(IList<Url> urls)
        {
            if (null == urls)
            {
                return;
            }

            StringBuilder list = new StringBuilder();

            foreach (Url url in urls)
            {
                list.AppendLine(url.Uri.ToString());
            }

            File.AppendAllText(this.filename, list.ToString()) ;
        }

    }
}
