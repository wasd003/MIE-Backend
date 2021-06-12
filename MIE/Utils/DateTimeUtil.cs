using System;
using System.Text.RegularExpressions;

namespace MIE.Utils
{
    public class DateTimeUtil
    {
        /**
         * date格式为MM/dd/yyyy
         */
        public static bool DateMatch(string date)
        {
            string pattern = "^(?<month>\\d{1,2})/(?<day>\\d{1,2})/(?<year>\\d{2,4})$";
            return Regex.IsMatch(date, pattern);
        }

        /**
         * dateStr的格式为:
         *  MM/dd/yyyy HH:mm:ss
         */
        public static string GetDateStr(DateTime date, DateTime time)
            => date.ToShortDateString() + " " + time.ToLongTimeString();

        public static string GetDateStr(string date, DateTime time)
        {
            if (!DateMatch(date)) throw new Exception("日期格式不正确");
            return date + " " + time.ToLongTimeString();
        }

        /**
         * 取date的date和time的time拼接成一个新的DateTime类型
         */
        public static DateTime ConcateDateTime(DateTime date, DateTime time)
            => DateTime.ParseExact(date.ToShortDateString() + " " + time.ToLongTimeString(),
                "MM/dd/yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
    }
}
