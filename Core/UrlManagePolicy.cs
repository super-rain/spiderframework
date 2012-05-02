/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示URL管理策略
    /// </summary>
    [Serializable]
    public sealed class UrlManagePolicy
    {
        /// <summary>
        /// 允许重复URL的规则
        /// </summary>
        [Serializable]
        public enum AllowRepeatUrlPolicies : byte
        {
            /// <summary>
            /// 不允许任何重复
            /// </summary>
            NoRepeat = 0x0,

            /// <summary>
            /// 允许索引URL重复
            /// </summary>
            OnlyIndex = 0x1,

            /// <summary>
            /// 允许最终URL重复
            /// </summary>
            OnlyFinal = 0x2,

            /// <summary>
            /// 允许所有重复
            /// </summary>
            All = OnlyIndex | OnlyFinal
        }

        private AllowRepeatUrlPolicies repeatPolicy;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UrlManagePolicy()
            : base()
        {
            this.repeatPolicy = AllowRepeatUrlPolicies.NoRepeat;
        }

        /// <summary>
        /// 获取或设置URL排重策略
        /// </summary>
        public AllowRepeatUrlPolicies UrlRepeatPolicy
        {
            get
            {
                return this.repeatPolicy;
            }
            set
            {
                this.repeatPolicy = value;
            }
        }

        /// <summary>
        /// 获取默认的策略
        /// </summary>
        public static UrlManagePolicy Default
        {
            get
            {
                UrlManagePolicy p = new UrlManagePolicy();
                return p;
            }
        }
    }
}
