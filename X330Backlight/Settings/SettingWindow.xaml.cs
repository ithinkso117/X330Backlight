using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace X330Backlight.Settings
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow
    {
        private readonly SettingViewModel _settingViewModel;

        public SettingWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += (s, e) =>
            {
                DragMove();
            };
            _settingViewModel = new SettingViewModel();
            DataContext = _settingViewModel;
        }


        public void ShowSettingWindow()
        {
            _settingViewModel.LoadSettings();
            Show();
        }


        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            _settingViewModel.SaveSettings();
            Hide();
        }
    }
}
