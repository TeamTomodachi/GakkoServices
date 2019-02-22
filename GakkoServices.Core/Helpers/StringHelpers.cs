using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GakkoServices.Core.Helpers
{
    public static class StringHelpers
    {
        public static bool IsNumeric(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }

        public static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static double SimilarTo(this string string1, string string2)
        {
            string[] splitString1 = string1.Split(' ');
            string[] splitString2 = string2.Split(' ');
            var strCommon = splitString1.Intersect(splitString2);
            //Formula : Similarity (%) = 100 * (CommonItems * 2) / (Length of String1 + Length of String2)
            double Similarity = (double)(100 * (strCommon.Count() * 2)) / (splitString1.Length + splitString2.Length);
            return Similarity;
        }
        
        /// <summary>
        /// Truncatess a given string to a given length
        /// </summary>
        /// <param name="value">The string to be truncated</param>
        /// <param name="maxLength">The maximum length of the string</param>
        /// <param name="trimCutWords">Whether the algorithm will trim out words which are cut in half</param>
        /// <returns></returns>
        public static string Truncate(this string value, int maxLength, bool trimCutWords = false)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            if (maxLength <= 0) return "";
            if (value.Length <= maxLength) return value;

            var outputString = value.Substring(0, maxLength);
            if (trimCutWords)
            {
                int endingWhiteSpace = outputString.LastIndexOf(' ');
                if (endingWhiteSpace == -1) return "";
                outputString = value.Substring(0, endingWhiteSpace);
            }

            return outputString;
        }
        
        public static string UrlCombine(string url1, string url2)
        {
            if (url1.Length == 0) { return url2; }
            if (url2.Length == 0) { return url1; }

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return string.Format("{0}/{1}", url1, url2);
        }
    }
}
