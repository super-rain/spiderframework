using System;
using System.Text;

namespace Souex.Spider.Framework.Helpers.Html
{
    internal class EncodingFoundException : Exception
    {
        private Encoding _encoding;

        internal EncodingFoundException(Encoding encoding)
        {
            _encoding = encoding;
        }

        internal Encoding Encoding
        {
            get
            {
                return _encoding;
            }
        }
    }
}
