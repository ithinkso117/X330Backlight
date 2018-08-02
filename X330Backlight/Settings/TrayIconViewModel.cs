using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace X330Backlight.Settings
{
    internal class TrayIconViewModel
    {
        private readonly string _name;

        public int Id { get; }

        public BitmapImage Image { get; }

        public TrayIconViewModel(string name, int id)
        {
            _name = name;
            Id = id;
            if (id != 0)
            {
                var imageUri = @"pack://application:,,,/"
                               + Assembly.GetExecutingAssembly().GetName().Name
                               + ";component/"
                               + $"Resources/TrayIcon{id}.png";
                Image = new BitmapImage(new Uri(imageUri, UriKind.Absolute));
            }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
