using System;

namespace Souex.Spider.Framework.Helpers.Html
{
    internal class NameValuePair
    {
        internal readonly string Name;
        internal string Value;

        internal NameValuePair()
        {
        }

        internal NameValuePair(string name)
            :
            this()
        {
            Name = name;
        }

        internal NameValuePair(string name, string value)
            :
            this(name)
        {
            Value = value;
        }
    }

}
