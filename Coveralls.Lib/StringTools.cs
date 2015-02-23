namespace Coveralls
{
    public static class StringTools
    {
        public static bool IsBlank(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotBlank(this string s)
        {
            return !s.IsBlank();
        }
    }
}