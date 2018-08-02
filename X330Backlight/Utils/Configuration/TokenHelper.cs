using System;

namespace X330Backlight.Utils.Configuration
{
    public class TokenHelper
    {
        private static readonly string[] EmptyStringArray = new string[0];

        /// <summary>
        ///  Splite the string by  '\t', '\r', '\n' .
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static string[] FromString(string tokens)
        {
            if (tokens == null) return EmptyStringArray;
            var values = tokens.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            return values;
        }

        /// <summary>
        ///  Splite the string according to the mask.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string[] FromString(string tokens, string mask)
        {
            if (tokens == null) return EmptyStringArray;
            return FromString(tokens, new[] { mask });
        }

        /// <summary>
        /// Splite the string according to the masks.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="masks"></param>
        /// <returns></returns>
        public static string[] FromString(string tokens, string[] masks)
        {
            if (tokens == null) return EmptyStringArray;
            var values = tokens.Split(masks, StringSplitOptions.RemoveEmptyEntries);

            return values;
        }

        /// <summary>
        /// Remove the comment which start with #
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DeleteComment(string text)
        {
            return text.Trim().StartsWith("#") ? string.Empty : text;
        }
    }
}
