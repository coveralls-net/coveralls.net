using System;
using System.Text;

namespace Symitar
{
    public static class Utilities
    {
        private static readonly string[] FileTypeDescriptor = { "RepWriter", "Letter", "Help", "Report" };
        private static readonly string[] FileFolder = { "REPWRITERSPECS", "LETTERSPECS", "HELPFILES", "REPORT" };

        public static DateTime ParseSystemTime(string date, string time)
        {
            if (string.IsNullOrEmpty(date))
                throw new ArgumentNullException("date", "Date cannot be blank.");

            int day, month, year, hour, minutes;

            if (string.IsNullOrEmpty(time))
            {
                hour = 0;
                minutes = 0;
            }
            else
            {
                while (time.Length < 4)
                {
                    time = '0' + time;
                }
                hour = Int32.Parse(time.Substring(0, 2));
                minutes = Int32.Parse(time.Substring(2, 2));
            }

            while (date.Length < 8)
                date = '0' + date;

            year = Int32.Parse(date.Substring(4, 4));
            month = Int32.Parse(date.Substring(0, 2));
            day = Int32.Parse(date.Substring(2, 2));

            return new DateTime(
                year,
                month,
                day,
                hour,
                minutes,
                0
                );
        }

        public static string FileTypeString(FileType type)
        {
            return FileTypeDescriptor[(int)type];
        }

        public static string ContainingFolder(int sym, FileType type)
        {
            if (sym < 0)
                throw new ArgumentOutOfRangeException("sym");

            return ContainingFolder(sym.ToString(), type);
        }

        public static string ContainingFolder(string sym, FileType type)
        {
            if (string.IsNullOrEmpty(sym)) throw new ArgumentNullException("sym");
            if (sym.Length > 3) throw new ArgumentOutOfRangeException("sym");

            sym = sym.PadLeft(3, '0');
            return String.Format("/SYM/SYM{0}/{1}", sym, FileFolder[(int)type]);
        }

        public static string DecodeString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static byte[] EncodeString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static int ConvertTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr))
                throw new ArgumentException("timeStr");

            string[] tokens = timeStr.Split(':');

            if (tokens.Length != 3)
                throw new ArgumentException("Invalid time format", "timeStr");

            string seconds = tokens[2], minutes = tokens[1], hours = tokens[0];

            int currTime = int.Parse(seconds);
            currTime += 60 * int.Parse(minutes);
            currTime += 3600 * int.Parse(hours);

            return currTime;
        }

        public static DateTime ConvertTime(int time)
        {
            if (time < 0)
                throw new ArgumentOutOfRangeException("Time cannot be negative.", "time");

            int hours = 0, minutes = 0, seconds = 0;

            while (time >= 3600)
            {
                hours++;
                time -= 3600;
            }

            while (time >= 60)
            {
                minutes++;
                time -= 60;
            }

            seconds = time;

            return new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                hours,
                minutes,
                seconds
            );
        }

        public static int ConvertTime(DateTime time)
        {
            int currTime = time.Second;
            currTime += 60 * time.Minute;
            currTime += 3600 * time.Hour;
            return currTime;
        }
    }
}