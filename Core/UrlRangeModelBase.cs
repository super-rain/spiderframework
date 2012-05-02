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
using System.Text.RegularExpressions;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 为规则模型提供基类
    /// </summary>
    [Serializable]
    public abstract class UrlRangeModelBase : IUrlRangeModel
    {
        private string urlTemplate;
        private string replaceMark;
        private bool isWellFormatTemplate;

        protected UrlRangeModelBase()
        {
            this.UrlTemplate = "";
            this.ReplaceMark = "";
            this.isWellFormatTemplate = false;
        }

        protected UrlRangeModelBase(string urlTemp, string mark)
        {
            this.UrlTemplate = urlTemp;
            this.ReplaceMark = mark;
        }

        protected UrlRangeModelBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.urlTemplate = info.GetString("urlTemplate");
            this.replaceMark = info.GetString("replaceMark");
            this.isWellFormatTemplate = info.GetBoolean("isWellFormatTemplate");
        }

        /// <summary>
        /// 使用的URL模板
        /// </summary>
        public string UrlTemplate
        {
            get
            {
                return this.urlTemplate;
            }
            set
            {
                this.urlTemplate = value;
                this.isWellFormatTemplate = Regex.IsMatch(this.urlTemplate, @"\{0\}{1}");
            }
        }

        /// <summary>
        /// 替换标记字符
        /// </summary>
        public string ReplaceMark
        {
            get
            {
                return this.replaceMark;
            }
            set
            {
                this.replaceMark = value;
            }
        }

        /// <summary>
        /// 获取一个值,指示当前的URL模板是否是格式良好的
        /// </summary>
        public bool IsWellFormatTemplate
        {
            get
            {
                return this.isWellFormatTemplate;
            }
        }

        /// <summary>
        /// 在派生类中重写此方法,用以生成URL列表
        /// </summary>
        /// <returns>string[]</returns>
        public string[] GenerateList()
        {
            ICollection<object> objects = new List<object>();
            this.GetRangeValues(objects);

            if (objects.Count == 0)
            {
                return null;
            }

            StringCollection sc = new StringCollection();
            string v = "";
            foreach (object obj in objects)
            {
                if (null != obj && obj.ToString()!="")
                {
                    if (this.isWellFormatTemplate)
                    {
                        v = String.Format(this.urlTemplate, obj);
                    }
                    else
                    {
                        v = this.urlTemplate.Replace(this.replaceMark, obj.ToString());
                    }
                    sc.Add(v);
                }
            }

            string[] list = new string[sc.Count];
            sc.CopyTo(list, 0);
            return list;
        }

        /// <summary>
        /// 在派生类中重写此方法,获取范围值集合
        /// </summary>
        /// <returns>ICollection</returns>
        protected abstract void GetRangeValues(ICollection<object> values);

        #region ISerializable 成员

        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("urlTemplate", this.urlTemplate);
            info.AddValue("replaceMark", this.replaceMark);
            info.AddValue("isWellFormatTemplate", this.isWellFormatTemplate);
        }

        #endregion
    }
}
