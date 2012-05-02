using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示内容处理程序集合
    /// </summary>
    [Serializable]
    public class ContentHandlerCollection:Collection<IContentHandler>
    {
        internal ContentHandlerCollection()
            :base()
        {
            //
        }
    }
}
