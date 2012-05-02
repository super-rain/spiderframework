using System;
using System.Collections.Generic;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 表示一个数字范围规则模型
    /// </summary>
    [Serializable]
    public class NumericUrlRangeModel : UrlRangeModelBase
    {
        private int range1;
        private int range2;
        private ushort zero;

        private NumericUrlRangeModel()
            : base()
        {
        }

        private NumericUrlRangeModel(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.range1 = info.GetInt32("range1");
            this.range2 = info.GetInt32("range2");
            this.zero = info.GetUInt16("zero");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="start">开始的数字</param>
        /// <param name="end">截止的数字</param>
        /// <param name="zeroPadding">填充前导零</param>
        /// <param name="template">URL模板,如果模板是良构的,替换字符可以为空</param>
        /// <param name="mark">替换字符</param>
        public NumericUrlRangeModel(int start, int end, ushort zeroPadding, string template, string mark)
            : base(template, mark)
        {
            if (end < start)
            {
                throw new ArgumentException("The end value of range should greater than start value,or equals at least!");
            }
            this.range1 = start;
            this.range2 = end;
            this.zero = zeroPadding;
        }


        /// <summary>
        /// 填充ICollection
        /// </summary>
        /// <param name="values">ICollection</param>
        protected override void GetRangeValues(ICollection<object> values)
        {
            for (int i = this.range1; i <= this.range2; i++)
            {
                values.Add(this.zero == 0 ? i.ToString() : i.ToString().PadLeft(this.zero, '0'));
            }
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("range1", this.range1);
            info.AddValue("range2", this.range2);
            info.AddValue("zero", this.zero);
        }
    }
}
