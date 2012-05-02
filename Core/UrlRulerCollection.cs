using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示URL规则集合
    /// </summary>
    [Serializable]
    public class UrlRulerCollection:Collection<UrlRuler>
    {
        internal UrlRulerCollection()
            : base()
        {
            //
        }
    }
}
