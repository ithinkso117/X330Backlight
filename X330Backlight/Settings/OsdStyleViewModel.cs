using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace X330Backlight.Settings
{
    internal class OsdStyleViewModel
    {
        private readonly string _name;

        public int StyleId { get; }

        public BitmapImage Image { get; }

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
            return _name;
        }
    }
}
