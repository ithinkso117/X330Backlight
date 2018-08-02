using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using X330Backlight.Services;
using X330Backlight.Services.Interfaces;
using X330Backlight.Utils;
using X330Backlight.Utils.NotifyIcon;

namespace X330Backlight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private HwndSource _hwndSource;
        private MainService _mainService;

        private readonly string _currentExeFilePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        private TaskbarIcon _taskbarIcon;
        private Bitmap _iconBitmap;

        private OsdWindow _osdWindow;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //Handle auto start.
            var appName = TranslateHelper.Translate("AppName");
            AutoStartHelper.AutoStart(appName, _currentExeFilePath, SettingManager.AutoStart);
            //Register the window handle created event handler.
            SourceInitialized += OnSourceInitialized;
        }

        /// <summary>
        /// Convert an image to bitmap.
        /// </summary>
        /// <param name="image">Image to convert.</param>
        /// <returns>The created bitmap.</returns>
        private Bitmap ImageToBitmap(ImageSource image)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image));
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Flush();
                return new Bitmap(stream);
            }
        }

        /// <summary>
        /// Start all functions.
        /// </summary>
        public void StartAllFunctions()
        {
            //For avoding duplicated start.
            StopAllFunctions();

            Logger.Write("Starting functions...");
            Logger.Write("Creating MainService...");
            _mainService = new MainService(_hwndSource);
            
            Logger.Write("Creating OSD window...");
            if (SettingManager.OsdStyle == 0)
            {
                _osdWindow = new DefaultOsdWindow();
            }
            else if (SettingManager.OsdStyle == 1)
            {
                _osdWindow = new CirculaOsdWindow();
            }

            if (SettingManager.TrayIconId > 0)
            {
                Logger.Write("Creating TrayIcon...");
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(_currentExeFilePath);
                var iconUri = @"pack://application:,,,/"
                              + Assembly.GetExecutingAssembly().GetName().Name
                              + ";component/"
                              + $"Resources/TrayIcon{SettingManager.TrayIconId}.png";
                try
                {
                    var iconImageUri = new Uri(iconUri, UriKind.Absolute);
                    var imageSource = new BitmapImage(iconImageUri);
                    _iconBitmap = ImageToBitmap(imageSource);
                    icon = System.Drawing.Icon.FromHandle(_iconBitmap.GetHicon());
                }
                catch (Exception ex)
                {
                    Logger.Write($"GetIcon from {iconUri} failed, error:{ex}");
                }
                
                _taskbarIcon = new TaskbarIcon { Icon = icon};
                var backlightService = ServiceManager.GetService<IBacklightService>();
                backlightService.BrightnessChanged += OnBrightnessChanged;
                OnBrightnessChanged(backlightService, EventArgs.Empty);
            }
            Logger.Write("All functions started.");
        }

        /// <summary>
        /// Close all functions.
        /// </summary>
        public void StopAllFunctions()
        {
            Logger.Write("Stopping functions...");
            if (_taskbarIcon != null)
            {
                var backlightService = ServiceManager.GetService<IBacklightService>();
                backlightService.BrightnessChanged -= OnBrightnessChanged;
                _taskbarIcon.Dispose();
                _taskbarIcon = null;
                _iconBitmap?.Dispose();
                _iconBitmap = null;
            }
            if (_osdWindow != null)
            {
                _osdWindow.Close();
                _osdWindow = null;
            }
            if (_mainService != null)
            {
                _mainService.Close();
                _mainService = null;
            }
            Logger.Write("All functions stopped.");
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
            {
                _hwndSource = hwndSource;
                Logger.Write("Starting application.");
                StartAllFunctions();
                Logger.Write("Application started.");
            }
        }

        private void OnBrightnessChanged(object sender, EventArgs e)
        {
            var backlightService = ServiceManager.GetService<IBacklightService>();
            var brightness = backlightService.Brightness;
            if (_taskbarIcon != null)
            {
                Dispatcher.Invoke(() =>
                {
                    _taskbarIcon.ToolTipText =
                        $"{TranslateHelper.Translate("CurrentBrightness")}: {brightness}";
                });

            }
        }


        protected override void OnClosed(EventArgs e)
        {
            StopAllFunctions();
            base.OnClosed(e);
        }
    }
}
