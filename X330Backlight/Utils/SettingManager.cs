using System;
using System.IO;
using System.Reflection;
using X330Backlight.Utils.Configuration;

namespace X330Backlight.Utils
{
    public class SettingManager
    {

        private static readonly string SettingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "X330Backlight.ini");


        public const int DefaultBrightness = 13;
        public const bool DefaultAutoStart = false;
        public const int DefaultOsdStyle = 1;
        public const int DefaultOsdTimeout = 3;
        public const int DefaultAcSavingModeTime = 300000;
        public const int DefaultAcSavingModeBrightness = 0;
        public const int DefaultBatterySavingModeTime = 180000;
        public const int DefaultBatterySavingModeBrightness = 0;
        public const int DefaultTurnOffMonitorWay = 1;
        public const int DefaultTrayIconId = 1;
        public const bool DefaultTurnOffMonitorByThinkVantage = true;
        public const bool DefaultReduceBrightnessWhenUsingBattery = true;
        public const bool DefaultReduceBrightnessWhenBatteryLow = true;

        private static readonly ConfigManager ConfigManager;

        /// <summary>
        /// Raised when the settings are updated, bool is need administrator rights.
        /// </summary>
        public static event EventHandler<bool> SettingsChanged;

        /// <summary>
        /// Gets if this time is first run.
        /// </summary>
        public static bool IsFirstRun { get;}

        /// <summary>
        /// Gets the version of the setting file.
        /// </summary>
        public static string Version { get; }

        /// <summary>
        /// Gets or sets the current brightness
        /// </summary>
        public static int Brightness { get; private set; }


        /// <summary>
        /// Gets or sets if the app will start automaticlly.
        /// </summary>
        public static bool AutoStart { get; private set; }


        /// <summary>
        /// Gets the OSD Style, 0 is notmal style, 1 is circula style.
        /// </summary>
        public static int OsdStyle { get; private set; }

        /// <summary>
        /// Gets the OSD window show timeout(unit is second).
        /// </summary>
        public static int OsdTimeout { get; private set; }

        /// <summary>
        /// Gets the time of enter AC saving mode(unit is ms).
        /// </summary>
        public static int AcSavingModeTime { get; private set; }

        /// <summary>
        /// Gets the brightness under the AC saving mode.
        /// </summary>
        public static int AcSavingModeBrightness { get; private set; }


        /// <summary>
        /// Gets the time of enter battery saving mode(unit is ms).
        /// </summary>
        public static int BatterySavingModeTime { get; private set; }

        /// <summary>
        /// Gets the brightness under the battery saving mode.
        /// </summary>
        public static int BatterySavingModeBrightness { get; private set; }

        /// <summary>
        /// Gets the TurnOffMonitor way from setting file.
        /// </summary>
        public static TurnOffMonitorWay TurnOffMonitorWay { get; private set; }

        /// <summary>
        /// Gets the trayicon's id, if is zero, trayicon will be disabled.
        /// </summary>
        public static int TrayIconId { get; private set; }

        /// <summary>
        /// Get if TurnOff the monitor when press ThinkVantage key.
        /// </summary>
        public static bool TurnOffMonitorByThinkVantage { get; private set; }


        /// <summary>
        /// Gets if reduce the brightness when switch to using battery.
        /// </summary>
        public static bool ReduceBrightnessWhenUsingBattery { get; private set; }

        /// <summary>
        /// Gets if reduce the brightness when battery is low.
        /// </summary>
        public static bool ReduceBrightnessWhenBatteryLow { get; private set; }


        static SettingManager()
        {
            var settingContent = string.Empty;
            if (File.Exists(SettingFilePath))
            {
                settingContent = File.ReadAllText(SettingFilePath);
            }
            ConfigManager = new ConfigManager(settingContent);
            Version = ConfigManager.GetValue("Setting", "Version", string.Empty);
            Brightness = ConfigManager.GetValue("Setting","Brightness", DefaultBrightness);
            AutoStart = ConfigManager.GetValue("Setting","AutoStart", DefaultAutoStart);
            OsdStyle = ConfigManager.GetValue("Setting", "OSDStyle", DefaultOsdStyle);
            OsdTimeout = ConfigManager.GetValue("Setting", "OSDTimeOut", DefaultOsdTimeout);
            AcSavingModeTime = ConfigManager.GetValue("Setting", "AcSavingModeTime", DefaultAcSavingModeTime);
            AcSavingModeBrightness = ConfigManager.GetValue("Setting", "AcSavingModeBrightness", DefaultAcSavingModeBrightness);
            BatterySavingModeTime = ConfigManager.GetValue("Setting", "BatterySavingModeTime", DefaultBatterySavingModeTime);
            BatterySavingModeBrightness = ConfigManager.GetValue("Setting", "BatterySavingModeBrightness", DefaultBatterySavingModeBrightness);
            ReduceBrightnessWhenUsingBattery = ConfigManager.GetValue("Setting", "ReduceBrightnessWhenUsingBattery", DefaultReduceBrightnessWhenUsingBattery);
            ReduceBrightnessWhenBatteryLow = ConfigManager.GetValue("Setting", "ReduceBrightnessWhenBatteryLow", DefaultReduceBrightnessWhenBatteryLow);
            TurnOffMonitorWay = ConfigManager.GetValue("Setting", "TurnOffMonitorWay", DefaultTurnOffMonitorWay) == 0? TurnOffMonitorWay.ZeroBrightness: TurnOffMonitorWay.TurnOff;
            TrayIconId = ConfigManager.GetValue("Setting", "TrayIconId", DefaultTrayIconId);
            TurnOffMonitorByThinkVantage = ConfigManager.GetValue("Setting", "TurnOffMonitorByThinkVantage", DefaultTurnOffMonitorByThinkVantage); 
            if (Version != Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                //Reset to default.
                ConfigManager = new ConfigManager(string.Empty);
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                Brightness = DefaultBrightness;
                UpdateSettings();
                IsFirstRun = true;
            }
        }

        /// <summary>
        /// Save the given brightness to the setting file.
        /// </summary>
        /// <param name="brightness"></param>
        public static void SaveBrightness(int brightness)
        {
            Brightness = brightness;
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "Brightness", Brightness);
            using (var fs = new FileStream(SettingFilePath,FileMode.Create, FileAccess.ReadWrite,FileShare.ReadWrite))
            {
                ((IConfigUpdater) ConfigManager).Save(fs);
            }
        }

        /// <summary>
        /// Update all settings
        /// </summary>
        /// <param name="autoStart">If Auto Start</param>
        /// <param name="osdStyle">The OSD style Id</param>
        /// <param name="osdTimeout">The OSD timeout time</param>
        /// <param name="acSavingModeTime">The time enter AC saving mode.</param>
        /// <param name="acSavingModeBrightness">The brightness of  AC saving mode.</param>
        /// <param name="batterySavingModeTime">The time enter Battery saving mode. </param>
        /// <param name="batterySavingModeBrightness">The brightness of  Battery saving mode.</param>
        /// <param name="reduceBrightnessWhenUsingBattery">If reduce the bright when battery is low</param>
        /// <param name="reduceBrightnessWhenBatteryLow">If reduce the bright when switch to using battery</param>
        /// <param name="turnOffMonitorWay">The way to turnoff the monitor.</param>
        /// <param name="trayIconId">The trayicon ID</param>
        /// <param name="turnOffMonitorByThinkVantage">If enable ThinkVantage to turn off/on the monitor.</param>
        public static void UpdateSettings(bool autoStart = DefaultAutoStart,
                                          int osdStyle = DefaultOsdStyle, 
                                          int osdTimeout = DefaultOsdTimeout, 
            int acSavingModeTime = DefaultAcSavingModeTime,
            int acSavingModeBrightness = DefaultAcSavingModeBrightness,
            int batterySavingModeTime = DefaultBatterySavingModeTime,
            int batterySavingModeBrightness = DefaultBatterySavingModeBrightness,
            bool reduceBrightnessWhenUsingBattery = DefaultReduceBrightnessWhenUsingBattery,
            bool reduceBrightnessWhenBatteryLow = DefaultReduceBrightnessWhenBatteryLow,
            TurnOffMonitorWay turnOffMonitorWay = (TurnOffMonitorWay)DefaultTurnOffMonitorWay, 
            int trayIconId = DefaultTrayIconId,
            bool turnOffMonitorByThinkVantage = DefaultTurnOffMonitorByThinkVantage)
        {
            AutoStart = autoStart;
            OsdStyle = osdStyle;
            OsdTimeout = osdTimeout;
            AcSavingModeTime = acSavingModeTime;
            AcSavingModeBrightness = acSavingModeBrightness;
            BatterySavingModeTime = batterySavingModeTime;
            BatterySavingModeBrightness = batterySavingModeBrightness;
            ReduceBrightnessWhenUsingBattery = reduceBrightnessWhenUsingBattery;
            ReduceBrightnessWhenBatteryLow = reduceBrightnessWhenBatteryLow;
            TurnOffMonitorWay = turnOffMonitorWay;
            TrayIconId = trayIconId;
            TurnOffMonitorByThinkVantage = turnOffMonitorByThinkVantage;

            ((IConfigUpdater)ConfigManager).SetValue("Setting", "Version", Version);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "Brightness", Brightness);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "AutoStart", AutoStart);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "OSDStyle", OsdStyle);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "OSDTimeOut", OsdTimeout);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "AcSavingModeTime", AcSavingModeTime);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "AcSavingModeBrightness", AcSavingModeBrightness);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "BatterySavingModeTime", BatterySavingModeTime);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "ReduceBrightnessWhenUsingBattery", reduceBrightnessWhenUsingBattery);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "ReduceBrightnessWhenBatteryLow", reduceBrightnessWhenBatteryLow);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "BatterySavingModeBrightness", BatterySavingModeBrightness);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "TurnOffMonitorWay", (int)TurnOffMonitorWay);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "TrayIconId", TrayIconId);
            ((IConfigUpdater)ConfigManager).SetValue("Setting", "TurnOffMonitorByThinkVantage", TurnOffMonitorByThinkVantage);
            using (var fs = new FileStream(SettingFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                ((IConfigUpdater)ConfigManager).Save(fs);
            }
        }

        /// <summary>
        /// Raiese a notification to notify everyone the settings are changed.
        /// </summary>
        public static void NotifySettingsChanged(bool requiredAdmin)
        {
            SettingsChanged?.Invoke(null, requiredAdmin);
        }
    }
}
