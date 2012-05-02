/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Web;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示一个URL求和算法的实现
    /// </summary>
    [Serializable]
    public sealed class GeneralUrlCheckSum : IUrlCheckSum
    {
        /// <summary>
        /// 忽略的参数集合
        /// </summary>
        private NameValueCollection ignoreParmasCollection;

        /// <summary>
        /// 指示是否忽略大小写
        /// </summary>
        private bool ignoreCase;

        /// <summary>
        /// 构造函数,默认大小写敏感
        /// </summary>
        internal GeneralUrlCheckSum()
        {
            this.ignoreCase = false;
            this.ignoreParmasCollection = new NameValueCollection();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        private GeneralUrlCheckSum(SerializationInfo info, StreamingContext context)
            : this()
        {
            this.ignoreCase = info.GetBoolean("ignoreCase");
            this.ignoreParmasCollection = info.GetValue("ignoreParmas", typeof(NameValueCollection)) as NameValueCollection;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="igCase">指示是否忽略大小写</param>
        public GeneralUrlCheckSum(bool igCase)
            : this()
        {
            this.ignoreCase = igCase;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="igCase">指示是否忽略大小写</param>
        /// <param name="igParams">指定忽略的参数集合</param>
        public GeneralUrlCheckSum(bool igCase, NameValueCollection igParams)
            : this(igCase)
        {
            this.ignoreParmasCollection = igParams;
        }

        #region IUrlCheckSum 成员

        /// <summary>
        /// 计算给定URL的校验和
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>uint</returns>
        public UInt32 CheckSum(Url url)
        {
            StringBuilder toCheck = new StringBuilder(url.Uri.Scheme + "://" + url.Uri.Host);
            if (!url.Uri.IsDefaultPort)
            {
                toCheck.Append(url.Uri.Port);
            }

            if (url.Uri.AbsolutePath != "/")
            {
                toCheck.Append(this.ignoreCase ? url.Uri.AbsolutePath.ToLower() : url.Uri.AbsolutePath);
            }

            if (url.Uri.Query.Length > 1)// >0 && !="?"
            {
                string query = url.Uri.Query.Substring(1);//Exclude the char "?"

                NameValueCollection nc = System.Web.HttpUtility.ParseQueryString(query);
                NameValueCollection nc2 = null;

                if (null != this.ignoreParmasCollection && this.ignoreParmasCollection.HasKeys())
                {
                    nc2 = new NameValueCollection();
                    if (nc.HasKeys())
                    {
                        foreach (string key in nc.Keys)
                        {
                            string ig = this.ignoreParmasCollection[key];//存在于忽略参数集合中
                            if (ig != "*" && ig != nc[key])
                            {
                                nc2.Add(key, nc[key]);
                            }
                        }
                    }
                }

                toCheck.Append('?');

                if (null != nc2 && nc2.HasKeys())
                {
                    foreach (string key in nc2.Keys)
                    {
                        toCheck.AppendFormat("{0}={1}&", key, nc2[key]);
                    }
                }
                else
                {
                    toCheck.Append(query);
                }
            }
            return CRC32.CheckSum(Utils.AmendUrlString(toCheck.ToString()));
        }

        /// <summary>
        /// 获取或设置要忽略的参数集合,设置{key=KeyName,value=*}将忽略所有的KeyName
        /// </summary>
        public NameValueCollection IgnoreParmasCollection
        {
            get
            {
                return this.ignoreParmasCollection;
            }
            set
            {
                this.ignoreParmasCollection = value;
            }
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ignoreCase", this.ignoreCase);
            info.AddValue("ignoreParmas", this.ignoreParmasCollection, typeof(NameValueCollection));
        }

        #endregion
    }
}
