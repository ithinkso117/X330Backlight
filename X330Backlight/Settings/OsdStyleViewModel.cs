using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using X330Backlight.Utils;

namespace X330Backlight.Settings
{
    internal class OsdStyleViewModel
    {
        private readonly string _name;

        /// <summary>
        /// Gets the OSD style Id, start from 1.
        /// </summary>
        public int StyleId { get; }

        /// <summary>
        /// Gets the OSD image preview.
        /// </summary>
        public BitmapImage Image { get; }

        /// <summary>
        /// Create the OsdStyleViewModel
        /// </summary>
        /// <param name="name">The name of the style, it can be translated.</param>
        /// <param name="styleId">The OSD style id, start from 1, scope is [1-2]</param>
        public OsdStyleViewModel(string name, int styleId)
        {
            _name = name;
            StyleId = styleId;
            var imageUri = @"pack://application:,,,/"
                          + Assembly.GetExecutingAssembly().GetName().Name
                          + ";component/"
                          + $"Resources/OSD{styleId}.png";
            Image = new BitmapImage(new Uri(imageUri, UriKind.Absolute));
        }

        public override string ToString()
        {
            return TranslateHelper.Translate(_name);
        }
    }
}
