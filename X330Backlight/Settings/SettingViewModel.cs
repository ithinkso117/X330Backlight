using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using X330Backlight.Annotations;
using X330Backlight.Utils;

namespace X330Backlight.Settings
{
    internal class SettingViewModel:INotifyPropertyChanged
    {
        private OsdStyleViewModel _selectedOsdStyles;
        private TrayIconViewModel _selectedTrayIcon;
        private bool _autoStart;
        private OsdTimeoutViewModel _selectedOsdTimeout;
        private TimeViewModel _selectedAcSavingModeTime;
        private TimeViewModel _selectedBatterySavingModeTime;
        private TurnOffMonitorWayViewModel _selectedTurnOffMonitorWay;
        private bool _enableThinkVantage;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool AutoStart
        {
            get => _autoStart;
            set
            {
                if (_autoStart != value)
                {
                    _autoStart = value;
                    OnPropertyChanged();
                }
            }
        }

        public IReadOnlyList<OsdStyleViewModel> OsdStyles { get; }

        public OsdStyleViewModel SelectedOsdStyles
        {
            get => _selectedOsdStyles;
            set
            {
                if (_selectedOsdStyles != value)
                {
                    _selectedOsdStyles = value;
                    OnPropertyChanged();
                }
            }
        }

        public IReadOnlyList<OsdTimeoutViewModel> OsdTimeouts { get; }

        public OsdTimeoutViewModel SelectedOsdTimeout
        {
            get => _selectedOsdTimeout;
            set
            {
                if (_selectedOsdTimeout != value)
                {
                    _selectedOsdTimeout = value;
                    OnPropertyChanged();
                }
            }
        }

        public IReadOnlyList<TimeViewModel> SavingModeTimes { get; }

        public TimeViewModel SelectedAcSavingModeTime
        {
            get => _selectedAcSavingModeTime;
            set
            {
                if (_selectedAcSavingModeTime != value)
                {
                    _selectedAcSavingModeTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeViewModel SelectedBatterySavingModeTime
        {
            get => _selectedBatterySavingModeTime;
            set
            {
                if (_selectedBatterySavingModeTime != value)
                {
                    _selectedBatterySavingModeTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public IReadOnlyList<TurnOffMonitorWayViewModel> TurnOffMonitorWays { get; }

        public TurnOffMonitorWayViewModel SelectedTurnOffMonitorWay
        {
            get => _selectedTurnOffMonitorWay;
            set
            {
                if (_selectedTurnOffMonitorWay != value)
                {
                    _selectedTurnOffMonitorWay = value;
                    OnPropertyChanged();
                }
            }
        }

        public IReadOnlyList<TrayIconViewModel> TrayIcons { get; }

        public TrayIconViewModel SelectedTrayIcon
        {
            get => _selectedTrayIcon;
            set
            {
                if (_selectedTrayIcon != value)
                {
                    _selectedTrayIcon = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EnableThinkVantage
        {
            get => _enableThinkVantage;
            set
            {
                if (_enableThinkVantage != value)
                {
                    _enableThinkVantage = value;
                    OnPropertyChanged();
                }
            }
        }

        public SettingViewModel()
        {
            OsdStyles = new List<OsdStyleViewModel>()
            {
                new OsdStyleViewModel(TranslateHelper.Translate("OSD1"),1),
                new OsdStyleViewModel(TranslateHelper.Translate("OSD2"),2)
            };

            OsdTimeouts = new List<OsdTimeoutViewModel>()
            {
                new OsdTimeoutViewModel(1),
                new OsdTimeoutViewModel(2),
                new OsdTimeoutViewModel(3),
                new OsdTimeoutViewModel(4),
                new OsdTimeoutViewModel(5),
            };

            SavingModeTimes = new List<TimeViewModel>()
            {
                new TimeViewModel(60000),
                new TimeViewModel(120000),
                new TimeViewModel(180000),
                new TimeViewModel(300000),
                new TimeViewModel(600000),
                new TimeViewModel(900000),
                new TimeViewModel(1200000),
                new TimeViewModel(1500000),
                new TimeViewModel(1800000),
                new TimeViewModel(2700000),
                new TimeViewModel(3600000),
                new TimeViewModel(7200000),
                new TimeViewModel(10800000),
                new TimeViewModel(14400000),
                new TimeViewModel(18000000),
                new TimeViewModel(0),
            };

            TurnOffMonitorWays = new List<TurnOffMonitorWayViewModel>()
            {
                new TurnOffMonitorWayViewModel(0),
                new TurnOffMonitorWayViewModel(1)
            };

            TrayIcons = new List<TrayIconViewModel>()
            {
                new TrayIconViewModel(TranslateHelper.Translate("TrayIcon1"),1),
                new TrayIconViewModel(TranslateHelper.Translate("TrayIcon2"),2),
                new TrayIconViewModel(TranslateHelper.Translate("TrayIcon3"),3),
                new TrayIconViewModel(TranslateHelper.Translate("TrayIcon4"),4),
                new TrayIconViewModel(TranslateHelper.Translate("DisableTrayIcon"),0),
            };
            LoadSettings();
        }

        public void LoadSettings()
        {
            AutoStart = SettingManager.AutoStart;
            SelectedOsdStyles = OsdStyles.First(x => x.StyleId == SettingManager.OsdStyle);
            SelectedOsdTimeout = OsdTimeouts.FirstOrDefault(x => x.Time == SettingManager.OsdTimeout);
            SelectedAcSavingModeTime = SavingModeTimes.First(x => x.Time == SettingManager.AcSavingModeTime);
            SelectedBatterySavingModeTime = SavingModeTimes.First(x => x.Time == SettingManager.BatterySavingModeTime);
            SelectedTurnOffMonitorWay = TurnOffMonitorWays.First(x => x.Way == (int)SettingManager.TrunOffMonitorWay);
            SelectedTrayIcon = TrayIcons.First(x => x.Id == SettingManager.TrayIconId);
            EnableThinkVantage = SettingManager.TrunOffMonitorByThinkVantage;
        }

        public void SaveSettings()
        {

        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
