using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using X330Backlight.Services;
using X330Backlight.Services.Interfaces;
using X330Backlight.Settings;
using X330Backlight.Utils;

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

        private NotifyIcon _notifyIcon;
        private int _currentTrayIconId;
        private Bitmap _settingBitmap;
        private Bitmap _exitBitmap;

        private SettingWindow _settingWindow;
        private OsdWindow _osdWindow;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SettingManager.SettingsChanged += OnSettingsChanged;
            HandleAutoStart();
        }



        //Handle auto start.
        private void HandleAutoStart()
        {
            var appName = TranslateHelper.Translate("AppName");
            AutoStartHelper.AutoStart(appName, _currentExeFilePath, SettingManager.AutoStart);
        }


        /// <summary>
        /// When settings changed, restart all functuons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsChanged(object sender, EventArgs e)
        {
            HandleAutoStart();
            StopAllFunctions();
            StartAllFunctions();
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
            Logger.Write("Starting functions...");
            Logger.Write("Creating MainService...");
            _mainService = new MainService(_hwndSource);

            Logger.Write("Creating OSD window...");
            if (SettingManager.OsdStyle == 1)
            {
                _osdWindow = new DefaultOsdWindow();
            }
            else if (SettingManager.OsdStyle == 2)
            {
                _osdWindow = new CirculaOsdWindow();
            }

            if (_notifyIcon == null)
            {
                Logger.Write("Creating TrayIcon...");
                var settingImageUri = @"pack://application:,,,/"
                                      + Assembly.GetExecutingAssembly().GetName().Name
                                      + ";component/"
                                      + "Resources/Setting.png";
                var exitImageUri = @"pack://application:,,,/"
                                   + Assembly.GetExecutingAssembly().GetName().Name
                                   + ";component/"
                                   + "Resources/Exit.png";
                var settingBitmapImage = new BitmapImage(new Uri(settingImageUri, UriKind.Absolute));
                var exitBitmapImage = new BitmapImage(new Uri(exitImageUri, UriKind.Absolute));

                _settingBitmap = ImageToBitmap(settingBitmapImage);
                _exitBitmap = ImageToBitmap(exitBitmapImage);
                _notifyIcon = new NotifyIcon
                {
                    ContextMenuStrip = new ContextMenuStrip()
                };
                _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(TranslateHelper.Translate("Setting"),
                    _settingBitmap, (s, e) => ShowSettingWindow()));
                _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(TranslateHelper.Translate("Exit"),
                    _exitBitmap, (s, e) => Close()));
                _notifyIcon.DoubleClick += OnNotifyIconDoubleClick;
            }

            _notifyIcon.Visible = SettingManager.TrayIconId > 0;
            if (_notifyIcon.Visible && _currentTrayIconId != SettingManager.TrayIconId)
            {
                _currentTrayIconId = SettingManager.TrayIconId;
                if (_notifyIcon.Icon != null)
                {
                    _notifyIcon.Icon.Dispose();
                    _notifyIcon.Icon = null;
                }
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(_currentExeFilePath);
                var iconUri = @"pack://application:,,,/"
                              + Assembly.GetExecutingAssembly().GetName().Name
                              + ";component/"
                              + $"Resources/TrayIcon{SettingManager.TrayIconId}.png";
                try
                {
                    var iconImageUri = new Uri(iconUri, UriKind.Absolute);
                    var imageSource = new BitmapImage(iconImageUri);
                    var iconBitmap = ImageToBitmap(imageSource);
                    icon = System.Drawing.Icon.FromHandle(iconBitmap.GetHicon());
                    iconBitmap.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Write($"GetIcon from {iconUri} failed, error:{ex}");
                }
                _notifyIcon.Icon = icon;

                var backlightService = ServiceManager.GetService<IBacklightService>();
                backlightService.BrightnessChanged += OnBrightnessChanged;
                OnBrightnessChanged(backlightService, EventArgs.Empty);
            }

            Logger.Write("All functions started.");
        }


        /// <summary>
        /// Show the setting window.
        /// </summary>
        private void ShowSettingWindow()
        {
            if (_settingWindow == null)
            {
                _settingWindow = new SettingWindow();
                _settingWindow.Closed += OnSettingWindowClosed;
            }
            _settingWindow.Show();
        }


        /// <summary>
        /// Handle double click trayicon event.
        /// </summary>
        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            ShowSettingWindow();
        }


        /// <summary>
        /// When setting window closed, set it to null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingWindowClosed(object sender, EventArgs e)
        {
            _settingWindow.Closed -= OnSettingWindowClosed;
            _settingWindow = null;
        }

        /// <summary>
        /// Close all functions.
        /// </summary>
        public void StopAllFunctions()
        {
            Logger.Write("Stopping functions...");
            if (_notifyIcon != null && _notifyIcon.Visible)
            {
                var backlightService = ServiceManager.GetService<IBacklightService>();
                backlightService.BrightnessChanged -= OnBrightnessChanged;
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

        protected override void OnWindowInitialized(HwndSource hwndSource)
        {
            _hwndSource = hwndSource;
            Logger.Write("Starting application.");
            StartAllFunctions();
            Logger.Write("Application started.");
            if (SettingManager.IsFirstRun)
            {
                if (_settingWindow == null)
                {
                    _settingWindow = new SettingWindow();
                    _settingWindow.Closed += OnSettingWindowClosed;
                }
                _settingWindow.Show();
            }
        }

        private void OnBrightnessChanged(object sender, EventArgs e)
        {
            if (_notifyIcon != null)
            {
                var backlightService = ServiceManager.GetService<IBacklightService>();
                var brightness = backlightService.Brightness;
                var displayText = $"{TranslateHelper.Translate("CurrentBrightness")}: {brightness}";
                Dispatcher.Invoke(() =>
                {
                    _notifyIcon.Text = displayText;
                });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _settingWindow?.Close();

            _notifyIcon?.Dispose();
            _notifyIcon = null;

            _exitBitmap?.Dispose();
            _exitBitmap = null;
            _settingBitmap?.Dispose();
            _settingBitmap = null;

            StopAllFunctions();
            base.OnClosed(e);
        }
    }
}
