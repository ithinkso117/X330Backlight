using System;
using System.Windows.Markup;

namespace X330Backlight.Utils
{
    [MarkupExtensionReturnType(typeof(string))]
    public class TranslaterExtension : MarkupExtension
    {
        #region Properties 

        [ConstructorArgument("key")]
        public string Key { get; set; }

        public string StringFormat { get; set; }

        #endregion Properties 

        #region Methods 

        // Constructors 

        public TranslaterExtension(string key)
        {
            Key = key;
        }
        
        // Methods 

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            string value = TranslateHelper.Translate(Key);
            if (!string.IsNullOrEmpty(StringFormat))
            {
                value = string.Format(TranslateHelper.Translate(StringFormat), value);
            }
            return value;
        }

        #endregion Methods 
    }
}