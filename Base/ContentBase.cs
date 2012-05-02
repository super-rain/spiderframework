using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Souex.Spider.Framework.Core;

namespace Souex.Spider.Framework.Base
{
    /// <summary>
    /// 内容处理基类
    /// </summary>
    public abstract class ContentBase:IDisposable
    {
        protected Content Content;

        private ContentBase()
        {
            this.Content = null;
        }

        protected ContentBase(Content content)
            :this()
        {
            this.Content = content;
        }



        #region IDisposable 成员

        public virtual void Dispose()
        {
            if (null != this.Content)
            {
                Content.Dispose();
            }
        }

        #endregion
    }
}
