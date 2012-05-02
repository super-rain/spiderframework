using System;
using System.Collections.Generic;
using System.Text;

using Souex.Spider.Framework.Core;

namespace Souex.Spider.Framework.Base
{
    /// <summary>
    /// 表示一个日期范围规则模型
    /// </summary>
    [Serializable]
    public class CalendricUrlRangeModel : UrlRangeModelBase
    {
        private DateTime startDate;
        private DateTime endDate;
        private string format;

        /// <summary>
        /// 构造函数
        /// </summary>
        private CalendricUrlRangeModel()
            : base()
        {
            this.startDate = DateTime.MinValue;
            this.endDate = DateTime.MinValue;
        }

        private CalendricUrlRangeModel(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.startDate = info.GetDateTime("startDate");
            this.endDate = info.GetDateTime("endDate");
            this.format = info.GetString("format");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="start">开始日期</param>
        /// <param name="end">截止日期</param>
        /// <param name="calendarFormat">日期格式</param>
        /// <param name="template">URL模板,如果模板是良构的,替换字符可以为空</param>
        /// <param name="mark">替换字符</param>
        public CalendricUrlRangeModel(DateTime start, DateTime end, string calendarFormat, string template, string mark)
            : base(template, mark)
        {
            this.startDate = start;
            this.endDate = end;
            this.format = calendarFormat;
        }

        /// <summary>
        /// 填充ICollection
        /// </summary>
        /// <param name="values">ICollection</param>
        protected override void GetRangeValues(ICollection<object> values)
        {

        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("startDate",this.startDate);
            info.AddValue("endDate", this.endDate);
            info.AddValue("format", this.format);
        }
    }
}
