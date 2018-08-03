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
        private SavingModeTimeViewModel _selectedAcSavingModeTime;
        private SavingModeTimeViewModel _selectedBatterySavingModeTime;
        private TurnOffMonitorWayViewModel _selectedTurnOffMonitorWay;
        private bool _enableThinkVantage;
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Gets or sets if auto start.
        /// </summary>
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

        /// <summary>
        /// Gets all available OsdStyles.
        /// </summary>
        public IReadOnlyList<OsdStyleViewModel> OsdStyles { get; }

        /// <summary>
        /// Gets or sets the selected OsdStyle.
        /// </summary>
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

        /// <summary>
        /// Gets all Osd timeout values.
        /// </summary>
        public IReadOnlyList<OsdTimeoutViewModel> OsdTimeouts { get; }


        /// <summary>
        /// Gets or sets the selected osd timeout.
        /// </summary>
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

        /// <summary>
        /// Gets all available saving mode times.
        /// </summary>
        public IReadOnlyList<SavingModeTimeViewModel> SavingModeTimes { get; }

        /// <summary>
        /// Gets or sets the selected AC saving mode time.
        /// </summary>
        public SavingModeTimeViewModel SelectedAcSavingModeTime
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

        /// <summary>
        /// Gets or sets the selected Battery saving mode time.
        /// </summary>
        public SavingModeTimeViewModel SelectedBatterySavingModeTime
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

        /// <summary>
        /// Gets all available TurnOffMonitorWays;
        /// </summary>
        public IReadOnlyList<TurnOffMonitorWayViewModel> TurnOffMonitorWays { get; }


        /// <summary>
        /// Gets or sets the selected TurnOffMonitorWay.
        /// </summary>
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


        /// <summary>
        /// Gets all available trayicons.
        /// </summary>
        public IReadOnlyList<TrayIconViewModel> TrayIcons { get; }

        /// <summary>
        /// Gets or sets the selected tray icon.
        /// </summary>
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


        /// <summary>
        /// Gets or sets if enable the ThinkVantage
        /// </summary>
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
                new OsdStyleViewModel("OSD1",1),
                new OsdStyleViewModel("OSD2",2)
            };

            OsdTimeouts = new List<OsdTimeoutViewModel>()
            {
                new OsdTimeoutViewModel(1),
                new OsdTimeoutViewModel(2),
                new OsdTimeoutViewModel(3),
                new OsdTimeoutViewModel(4),
                new OsdTimeoutViewModel(5),
            };

            SavingModeTimes = new List<SavingModeTimeViewModel>()
            {
                new SavingModeTimeViewModel(60000),
                new SavingModeTimeViewModel(120000),
                new SavingModeTimeViewModel(180000),
                new SavingModeTimeViewModel(300000),
                new SavingModeTimeViewModel(600000),
                new SavingModeTimeViewModel(900000),
                new SavingModeTimeViewModel(1200000),
                new SavingModeTimeViewModel(1500000),
                new SavingModeTimeViewModel(1800000),
                new SavingModeTimeViewModel(2700000),
                new SavingModeTimeViewModel(3600000),
                new SavingModeTimeViewModel(7200000),
                new SavingModeTimeViewModel(10800000),
                new SavingModeTimeViewModel(14400000),
                new SavingModeTimeViewModel(18000000),
                new SavingModeTimeViewModel(0),
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


        /// <summary>
        /// Initialize all properties from SettingManager.
        /// </summary>
        private void LoadSettings()
        {
            AutoStart = SettingManager.AutoStart;
            SelectedOsdStyles = OsdStyles.First(x => x.StyleId == SettingManager.OsdStyle);
            SelectedOsdTimeout = OsdTimeouts.FirstOrDefault(x => x.Time == SettingManager.OsdTimeout);
            SelectedAcSavingModeTime = SavingModeTimes.First(x => x.Time == SettingManager.AcSavingModeTime);
            SelectedBatterySavingModeTime = SavingModeTimes.First(x => x.Time == SettingManager.BatterySavingModeTime);
            SelectedTurnOffMonitorWay = TurnOffMonitorWays.First(x => x.Way == (int)SettingManager.TurnOffMonitorWay);
            SelectedTrayIcon = TrayIcons.First(x => x.Id == SettingManager.TrayIconId);
            EnableThinkVantage = SettingManager.TurnOffMonitorByThinkVantage;
        }

        /// <summary>
        /// Save all settings and reload them.
        /// </summary>
        public void SaveSettings()
        {
            SettingManager.UpdateSettings(
                AutoStart, 
                SelectedOsdStyles.StyleId, 
                SelectedOsdTimeout.Time, 
                SelectedAcSavingModeTime.Time,
                batterySavingModeTime:SelectedBatterySavingModeTime.Time,
                turnOffMonitorWay:(TurnOffMonitorWay)SelectedTurnOffMonitorWay.Way,
                trayIconId:SelectedTrayIcon.Id,
                turnOffMonitorByThinkVantage:EnableThinkVantage
                );
            //Notify the main app to reload the settings.
            SettingManager.NotifySettingsChanged();
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
