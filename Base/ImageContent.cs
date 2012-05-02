using System;
using System.Collections.Generic;
using System.Text;

using Souex.Spider.Framework.Core;

namespace Souex.Spider.Framework.Base
{
    /// <summary>
    /// 提供图片内容的处理
    /// </summary>
    public class ImageContent : ContentBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="binary"></param>
        public ImageContent(BinaryContent binary)
            : base(binary)
        {
            //
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
