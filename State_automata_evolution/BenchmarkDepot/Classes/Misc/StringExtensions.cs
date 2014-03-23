using System;

namespace BenchmarkDepot.Classes.Extensions
{

    public static class StringExtensions
    {

        /// <summary>
        /// Simple extension method that trims a string to a given size -
        /// if it's too long replaces the redundant characters with "..."
        /// </summary>
        /// <param name="s">the string to trim</param>
        /// <param name="length">the desired length</param>
        public static string SetStringToLenght(this string s, int length)
        {
            return s.Length > length ? s.Substring(0, length - 3) + "..." : s;
        }

    }

}
