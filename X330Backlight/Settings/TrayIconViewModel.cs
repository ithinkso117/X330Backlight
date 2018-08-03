using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace X330Backlight.Settings
{
    internal class TrayIconViewModel
    {

        /// <summary>
        /// Gets the name of the trayicon.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Gets the id of the tray icon the scope is [0-4], 0 is no trayicon.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the trayicon preview image.
        /// </summary>
        public BitmapImage Image { get; }

        /// <summary>
        /// Create the TrayIconViewModel
        /// </summary>
        /// <param name="name">The name of the trayicon</param>
        /// <param name="id">The id of the tray icon the scope is [0-4], 0 is no trayicon.</param>
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
