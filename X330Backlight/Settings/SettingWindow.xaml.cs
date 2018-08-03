using System.Windows;

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


        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            _settingViewModel.SaveSettings();
            Close();
        }
    }
}
