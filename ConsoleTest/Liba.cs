using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Souex.Spider.Framework.Core;
using Souex.Spider.Framework.Helpers.Html;
using Souex.Spider.Framework.Helpers.Data;


namespace ConsoleTest
{
    class Liba
    {
        static void Main(string[] args)
        {
            //Regex mReg = new Regex(@"<a\s+href=""member\.php\?uid=(?<ID>\d+)""[^>]*>\s*(?<NAME>[^<>]+)\s*</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //string content = System.IO.File.ReadAllText(@"e:\f_545.txt");
            //MatchCollection matches = mReg.Matches(content.ToString());
            //foreach (Match m in matches)
            //{
            //    if (!m.Success)
            //    {
            //        continue;
            //    }
            //    string uid = m.Groups["ID"].Value;
            //    string uname = m.Groups["NAME"].Value;
            //    //if (!list.ContainsKey(uid))
            //    //{
            //    //    list.Add(uid, uname);
            //    //}

            //    Console.WriteLine("ID={0},Name='{1}'", uid, uname);
            //}


            //MySqlHelper db = DbHelper.CreateMySqlHelper("host=localhost;database=liba_user;uid=root;pwd=123456;charset=utf8") as MySqlHelper;
            //using (System.Data.Common.DbDataReader reader = db.ExecuteReader("SELECT uid,uname from users order by uid asc",System.Data.CommandBehavior.CloseConnection))
            //{
            //    while (reader.Read())
            //    {
            //        //FinalUrl url = Url.CreateFinalUrl(String.Format(""), null);
            //        Console.WriteLine("UID={0},UNAME={1}",reader.GetInt64(0),reader.GetString(1));
            //    }
            //}
            //while (true)
            //{
            //    Thread.Sleep(200);
            //}

            SpiderBase spider = new LibaSpider(new LibaSetting2(), new FileBasedUrlManager(@".\liba.urls"));
            spider.Start();

            while (spider.Status == SpiderBase.SpiderRunStatus.Running)
            {
                Thread.Sleep(200);
            }
        }

    }

    class LibaSpider : SpiderBase
    {
        private long maxMemoSize;
        public LibaSpider(SpiderSetting st, UrlManager m)
            : base(st, m)
        {
            maxMemoSize = 0;
        }

        protected override void SetConsoleTitle()
        {
            if (!Environment.UserInteractive)
            {
                return;
            }

            this.maxMemoSize = Math.Max(this.maxMemoSize, GC.GetTotalMemory(false));
            SpiderRuntime sr = this.Runtime;
            long avgBytes = sr.Seconds <= 0 ? 0 : Convert.ToInt64((double)sr.BytesLoaded / sr.Seconds);
            string title = String.Format("{0} - 线程:{1}|{2},队列:{3}/{4},{5} of {13},耗时:{6:0.000}秒,下载:{7},Avg:{8}/S,发送:{9},内存:{10}/MAX:{11} - {12}",
                this.Status,
                sr.CrawlThreads,
                sr.ProcessThreads,
                sr.UrlQueueLength,
                sr.ContentQueueLength,
                sr.UrlCount,
                sr.Seconds,
                Utils.GetBytesFriendly(sr.BytesLoaded),
                Utils.GetBytesFriendly(avgBytes, "0"),
                Utils.GetBytesFriendly(sr.BytesSent),
                Utils.GetBytesFriendly(sr.MemorySize),
                Utils.GetBytesFriendly(this.maxMemoSize),
                this.Settings.Name,
                sr.UrlTotal
            );
            Console.Title = title;
        }

        protected override void BeforeStarting()
        {
            MySqlHelper db = DbHelper.CreateMySqlHelper("host=localhost;database=liba_user;uid=root;pwd=123456;charset=utf8") as MySqlHelper;

            using (System.Data.Common.DbDataReader reader = db.ExecuteReader("SELECT uid,uname from users where email='' order by uid asc", System.Data.CommandBehavior.CloseConnection))
            {
                //and uname REGEXP('^[0-9a-z_-]+$')
                while (reader.Read())
                {
                    string uid = reader.GetInt64(0).ToString();
                    string uname = reader.GetString(1).Trim();
                    FinalUrl url = Url.CreateFinalUrl(String.Format("http://bbs.sh.libaclub.com/sendpass.php?action=sendme&username1={0}&id={1}", System.Web.HttpUtility.UrlEncode(Encoding.Default.GetBytes(uname)), uid), null) as FinalUrl;
                    url.UriEscape = false;
                    if (null != url)
                    {
                        url.ContentHandlers.Add(new Libahander2());
                        this.ManualQueue(url, false);
                    }
                }
            }
        }

        
    }

    class LibaSetting : SpiderSetting
    {
        public LibaSetting()
            : base()
        {
            this.StartUrl = new StartUrl("http://bbs.sh.libaclub.com/f_357.htm");
            this.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)";
            this.CrawlThreads = 5;
            this.ProcessThreads = 2;
            this.SpeedMode = SpiderSetting.SpeedModes.Normal;
            this.RequestEncoding = Encoding.Default;
            this.UrlExtractor = new HtmlUrlExtractor(true, false);
            this.UrlExtractor.OnlySameDomain = true;
            this.UrlExtractor.OnlySameHost = true;
            this.UrlExtractor.ExtractFinal = true;
            Regex itemListRegex = new Regex(@"^f_357_-1_\d+\.htm$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            UrlRuler itemListRuler = UrlRuler.CreateRegexRuler(itemListRegex, UrlTypes.Final);
            itemListRuler.ContentHandlers.Add(new LibaHandler1());
            this.UrlExtractor.Rulers.Add(itemListRuler);
        }
    }

    class LibaSetting2 : SpiderSetting
    {
        public LibaSetting2()
            : base()
        {
            //this.StartUrl = new StartUrl("http://bbs.sh.libaclub.com/f_357.htm");
            this.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)";
            this.CrawlThreads = 5;
            this.ProcessThreads = 1;
            this.SpeedMode = SpiderSetting.SpeedModes.Normal;
            this.RequestEncoding = Encoding.Default;
            this.UrlExtractor = new HtmlUrlExtractor(true, false);
            this.UrlExtractor.OnlySameDomain = true;
            this.UrlExtractor.OnlySameHost = true;
            this.UrlExtractor.ExtractFinal = true;

            //Regex itemListRegex = new Regex(@"^f_357_-1_\d+\.htm$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //UrlRuler itemListRuler = UrlRuler.CreateRegexRuler(itemListRegex, UrlTypes.Final);
            //itemListRuler.ContentHandlers.Add(new LibaHandler1());
            //this.UrlExtractor.Rulers.Add(itemListRuler);
        }
    }

    //提取用户
    class LibaHandler1 : IContentHandler
    {
        #region IContentHandler 成员

        //<a href="member.php?uid=470515" class="black" target="_blank">琥珀的天空 </a>
        private static Regex mReg = new Regex(@"<a\s+href=""member\.php\?[^""]*uid=(?<ID>\d+)[^""]*""[^>]*>\s*(?<NAME>[^<>]+)\s?</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static MySqlHelper db = DbHelper.CreateMySqlHelper("host=localhost;database=liba_user;uid=root;pwd=123456;charset=utf8") as MySqlHelper;

        public void Process(Content content, ProcessContext context)
        {
            if (!(content is TextContent))
            {
                return;
            }

            //content.SaveToFile(Utils.GetAppPath() +"DATA\\"+ content.RawUrl.CheckSum+".txt");

            StringDictionary list = new StringDictionary();
            string uid, uname;

            MatchCollection matches = mReg.Matches(((TextContent)content).Content);
            foreach (Match m in matches)
            {
                if (!m.Success)
                {
                    continue;
                }
                uid = m.Groups["ID"].Value;
                uname = m.Groups["NAME"].Value;
                if (!String.IsNullOrEmpty(uid) && !list.ContainsKey(uid) && !String.IsNullOrEmpty(uname))
                {
                    list.Add(uid, uname);
                    Console.WriteLine("ID={0},Name='{1}'", uid, uname);
                }
            }

            if (list.Count == 0)
            {
                return;
            }

            foreach (string k in list.Keys)
            {
                string sql = String.Format("select COUNT(`uid`) from users where uid={0:d}", k);
                int n = Convert.ToInt32(db.ExecuteScalar(sql));
                if (n > 0)
                {
                    continue;
                }

                sql = String.Format("INSERT INTO users (uid, uname, email)VALUES('{0:d}', '{1}', '');", k, list[k]);
                db.ExecuteNonQuery(sql);
            }
        }

        public int Priority
        {
            get
            {
                return 1;
            }
            set
            {
                //
            }
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            //
        }

        #endregion
    }

    //提取Email
    class Libahander2 : IContentHandler
    {
        //Email
        private static Regex mReg = new Regex(@"(\w+)([\-+.][\w]+)*@(\w[\-\w]*\.){1,5}([A-Za-z]){2,6}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static MySqlHelper db = DbHelper.CreateMySqlHelper("host=localhost;database=liba_user;uid=root;pwd=123456;charset=utf8") as MySqlHelper;
        #region IContentHandler 成员

        public void Process(Content content, ProcessContext context)
        {
            if (!(content is TextContent))
            {
                return;
            }

            content.SaveToFile(@"E:\SOUEX\tmp\" + content.RawUrl.GetRawParam("id")+".txt");

            Console.WriteLine("提取Email From: {0}", content.RawUrl.GetUrl());
            Match m = mReg.Match(((TextContent)content).Content);

            if (null != m && m.Success)
            {
                string sql = String.Format("UPDATE users SET email='{0}' WHERE uid='{1}'", m.Value, content.RawUrl.GetRawParam("id"));
                Console.WriteLine(sql);
                db.ExecuteNonQuery(sql);
            }
        }

        public int Priority
        {
            get
            {
                return 1;
            }
            set
            {
                //
            }
        }

        #endregion

        #region ISerializable 成员

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
