using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Net;

using Souex.Spider.Framework.Core;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Url u = Url.CreateIndexUrl("http://www.sina.com.cn", "/usr.php?u=我是谁");
            u.UriEscape = false;
            Console.WriteLine(u.GetUrl());
            Console.Read();
        }
    }


}
