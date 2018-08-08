using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using X330Backlight.Services;
using X330Backlight.Services.Interfaces;
using X330Backlight.Settings;
using X330Backlight.TaskbarIcons;
using X330Backlight.TaskbarIcons.TaskbarIcon;
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

        private TaskbarIcon _taskbarIcon;
        private SettingWindow _settingWindow;
        private OsdWindow _osdWindow;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SettingManager.SettingsChanged += OnSettingsChanged;
        }



        //Handle auto start.
        private void HandleAutoStart()
        {
            var appName = TranslateHelper.Translate("AppName");
            var auto = SettingManager.AutoStart ? 1 : 0;
            var helper = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoStart.exe");
            if (File.Exists(helper))
            {
                var process = Process.Start(new ProcessStartInfo(helper)
                {
                    Arguments = $"/name=\"{appName}\" /file=\"{_currentExeFilePath}\" /auto=\"{auto}\"",
                    CreateNoWindow = false,
                    Verb = "runas",
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                });
                process?.WaitForExit();
            }
        }


        /// <summary>
        /// When settings changed, restart all functuons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsChanged(object sender, bool e)
        {
            if (e)
            {
                HandleAutoStart();
            }

            StopAllFunctions();
            StartAllFunctions();
        }

        /// <summary>
        /// Convert an image to bitmap.
        /// </summary>
        /// <param name="image">Image to convert.</param>
        /// <returns>The created bitmap.</returns>
        private Bitmap ImageToBitmap(BitmapImage image)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
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

            if (_taskbarIcon == null && SettingManager.TrayIconId > 0)
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
                    var image = new BitmapImage(iconImageUri);
                    var iconBitmap = ImageToBitmap(image);
                    icon = System.Drawing.Icon.FromHandle(iconBitmap.GetHicon());
                    iconBitmap.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Write($"GetIcon from {iconUri} failed, error:{ex}");
                }

                var settingCommand = new TaskbarIconMenuCommand(ShowSettingWindow);
                var exitCommand = new TaskbarIconMenuCommand(Close);
                var taskbarIconViewModel = new TaskbarIconViewModel(settingCommand,exitCommand);
                _taskbarIcon = new TaskbarIcon
                {
                    DataContext = taskbarIconViewModel,
                    ContextMenu = Application.Current.TryFindResource("SysTrayMenu") as ContextMenu,
                    TrayToolTip = Application.Current.TryFindResource("SysTrayToolTip") as Border,
                    Icon = icon
                };
                _taskbarIcon.TrayMouseDoubleClick += OnTrayIconDoubleClick;
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
        private void OnTrayIconDoubleClick(object sender, RoutedEventArgs e)
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
            if (_taskbarIcon != null)
            {
                _taskbarIcon.TrayMouseDoubleClick -= OnTrayIconDoubleClick;
                var backlightService = ServiceManager.GetService<IBacklightService>();
                backlightService.BrightnessChanged -= OnBrightnessChanged;
                _taskbarIcon.Dispose();
                _taskbarIcon = null;
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
            if (_taskbarIcon != null)
            {
                var backlightService = ServiceManager.GetService<IBacklightService>();
                var brightness = backlightService.Brightness;
                var displayText = $"{TranslateHelper.Translate("CurrentBrightness")}: {brightness}";
                Dispatcher.Invoke(() =>
                {
                    if (((Border) _taskbarIcon.TrayToolTip)?.Child is TextBlock textBlock)
                    {
                        textBlock.Text = displayText;
                    }
                });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _settingWindow?.Close();
            StopAllFunctions();
            base.OnClosed(e);
        }
    }
}
