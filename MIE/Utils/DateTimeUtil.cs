using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

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
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("");
            return date.ToShortDateString() + " " + time.ToLongTimeString();
        }

        /**
         * dateStr的格式为:
         *  MM/dd/yyyy HH:mm:ss
         */
        public static string GetDateStr(string date, DateTime time)
        {
            if (!DateMatch(date)) throw new Exception("日期格式不正确");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("");
            DateTime dateInDateTime = DateTime.ParseExact(date, "M/d/yyyy", CultureInfo.InvariantCulture);
            return dateInDateTime.ToShortDateString() + " " + time.ToLongTimeString();
        }

        /**
         * 取date的date和time的time拼接成一个新的DateTime类型
         */
        public static DateTime ConcateDateTime(DateTime date, DateTime time)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("");
            string s = date.ToShortDateString() + " " + time.ToLongTimeString();
            Console.WriteLine(Thread.CurrentThread.CurrentCulture);
            Console.WriteLine("before extract:" + s);
            return DateTime.ParseExact(s, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
