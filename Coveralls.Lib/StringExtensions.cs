namespace Coveralls.Lib
{
    public static class StringExtensions
    {
        public static bool IsBlank(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotBlank(this string s)
        {
            return !s.IsBlank();
        }

        public static string ToUnixPath(this string path)
        {
            return path.Replace('\\', '/');
        }
    }
}