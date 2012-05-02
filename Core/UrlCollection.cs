using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示Url集合
    /// </summary>
    [Serializable]
    internal class UrlCollection : Collection<Url>
    {
        public UrlCollection()
            : base()
        {
            //
        }
    }
}
