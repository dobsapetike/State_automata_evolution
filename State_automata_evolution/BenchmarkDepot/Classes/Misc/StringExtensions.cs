using System;

namespace BenchmarkDepot.Classes.Extensions
{

    public static class StringExtensions
    {
        public static string SetStringToLenght(this string s, int length)
        {
            return s.Length > length ? s.Substring(0, length - 3) + "..." : s;
        }
    }

}
