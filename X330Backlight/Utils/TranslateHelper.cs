using System;
using System.Globalization;
using System.IO;
using X330Backlight.Utils.Configuration;

namespace X330Backlight.Utils
{
    public class TranslateHelper
    {
        public const string DefaultLanguage = "en-US";

        private static readonly ConfigManager LanguageManager;


        // Constructors 
        static TranslateHelper()
        {
            var currentLanguage = CultureInfo.CurrentCulture.Name.ToLower();
            var languageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");
            var languageFile = Path.Combine(languageDir , currentLanguage + ".ini");
            if (!File.Exists(languageFile))
            {
                languageFile = Path.Combine(languageDir, DefaultLanguage + ".ini");
            }
            LanguageManager = new ConfigManager(File.ReadAllText(languageFile));
        }

        // Methods 


        /// <summary>
        /// Translate to target Language with specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Translate(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            var translatedValue = LanguageManager?.GetValue("Language", key, string.Empty);
            if (string.IsNullOrEmpty(translatedValue))
            {
                return key;
            }
            return translatedValue.Replace("\\r\\n",Environment.NewLine);
        }
    }
}
