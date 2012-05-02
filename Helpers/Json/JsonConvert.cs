/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Souex.Spider.Framework.Helpers.Json
{
    /// <summary>
    /// 实现JSON格式数据的序列化和反序列化
    /// </summary>
    public static class JsonConvert
    {
        #region static

        private static JsonObject _json = new JsonObject();//寄存器
        private static readonly string _SEMICOLON = ":";//分号转义符
        private static readonly string _COMMA = ","; //逗号转义符

        #endregion

        #region escape
        /// <summary>
        /// 字符串转义,将双引号内的:和,分别转成_SEMICOLON和_COMMA
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string StrEncode(string text)
        {
            MatchCollection matches = Regex.Matches(text, "\\\"[^\\\"]*\\\"");
            foreach (Match match in matches)
            {
                text = text.Replace(match.Value, match.Value.Replace(":", _SEMICOLON).Replace(",", _COMMA));
            }

            return text;
        }

        /// <summary>
        /// 字符串转义,将_SEMICOLON和_COMMA分别转成:和,
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string StrDecode(string text)
        {
            return text.Replace(_SEMICOLON, ":").Replace(_COMMA, ",");
        }

        #endregion

        #region private methods

        /// <summary>
        /// 最小对象转为JSONObject
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static JsonObject DeserializeSingletonObject(string text)
        {
            JsonObject jsonObject = new JsonObject();

            MatchCollection matches = Regex.Matches(text, "(\\\"(?<key>[^\\\"]+)\\\":\\\"(?<value>[^,\\\"]*)\\\")|(\\\"(?<key>[^\\\"]+)\\\":(?<value>[^,\\\"\\}]*))");
            foreach (Match match in matches)
            {
                string value = match.Groups["value"].Value;
                jsonObject.Add(match.Groups["key"].Value, _json.ContainsKey(value) ? _json[value] : StrDecode(value));
            }

            return jsonObject;
        }

        /// <summary>
        /// 最小数组转为JSONArray
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static JsonArray DeserializeSingletonArray(string text)
        {
            JsonArray jsonArray = new JsonArray();

            MatchCollection matches = Regex.Matches(text, "(\\\"(?<value>[^,\\\"]+)\")|(?<value>[^,\\[\\]]+)");
            foreach (Match match in matches)
            {
                string value = match.Groups["value"].Value;
                jsonArray.Add(_json.ContainsKey(value) ? _json[value] : StrDecode(value));
            }

            return jsonArray;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string Deserialize(string text)
        {
            _json.Clear();//2010-01-20 清空寄存器
            text = StrEncode(text);//转义;和,

            int count = 0;
            string key = string.Empty;
            string pattern = "(\\{[^\\[\\]\\{\\}]*\\})|(\\[[^\\[\\]\\{\\}]*\\])";

            while (Regex.IsMatch(text, pattern))
            {
                MatchCollection matches = Regex.Matches(text, pattern);
                foreach (Match match in matches)
                {
                    key = "___key" + count + "___";

                    if (match.Value.Substring(0, 1) == "{")
                        _json.Add(key, DeserializeSingletonObject(match.Value));
                    else
                        _json.Add(key, DeserializeSingletonArray(match.Value));

                    text = text.Replace(match.Value, key);

                    count++;
                }
            }
            return text;
        }

        #endregion

        #region public methods

        /// <summary>
        /// 反序列化JSONObject对象
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static JsonObject DeserializeObject(string text)
        {
            return _json[Deserialize(text)] as JsonObject;
        }

        /// <summary>
        /// 反序列化JSONArray对象
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static JsonArray DeserializeArray(string text)
        {
            return _json[Deserialize(text)] as JsonArray;
        }

        /// <summary>
        /// 序列化JSONObject对象
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static string SerializeObject(JsonObject jsonObject)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (KeyValuePair<string, object> kvp in jsonObject)
            {
                if (kvp.Value is JsonObject)
                {
                    sb.Append(string.Format("\"{0}\":{1},", kvp.Key, SerializeObject((JsonObject)kvp.Value)));
                }
                else if (kvp.Value is JsonArray)
                {
                    sb.Append(string.Format("\"{0}\":{1},", kvp.Key, SerializeArray((JsonArray)kvp.Value)));
                }
                else if (kvp.Value is String)
                {
                    sb.Append(string.Format("\"{0}\":\"{1}\",", kvp.Key, kvp.Value));
                }
                else
                {
                    sb.Append(string.Format("\"{0}\":\"{1}\",", kvp.Key, ""));
                }
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 序列化JSONArray对象
        /// </summary>
        /// <param name="jsonArray"></param>
        /// <returns></returns>
        public static string SerializeArray(JsonArray jsonArray)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < jsonArray.Count; i++)
            {
                if (jsonArray[i] is JsonObject)
                {
                    sb.Append(string.Format("{0},", SerializeObject((JsonObject)jsonArray[i])));
                }
                else if (jsonArray[i] is JsonArray)
                {
                    sb.Append(string.Format("{0},", SerializeArray((JsonArray)jsonArray[i])));
                }
                else if (jsonArray[i] is String)
                {
                    sb.Append(string.Format("\"{0}\",", jsonArray[i]));
                }
                else
                {
                    sb.Append(string.Format("\"{0}\",", ""));
                }

            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }
        #endregion
    }
}
