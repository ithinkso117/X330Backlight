using System;
using System.IO;
using System.Text;
using X330Backlight.Utils.Configuration;

namespace X330Backlight.Utils
{
    public class SettingManager
    {

        private static readonly string SettingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "X330Backlight.ini");


        public const int DefaultBrightness = 13;
        public const bool DefaultAutoStart = false;
        public const int DefaultOsdStyle = 0;
        public const int DefaultOsdTimeout = 3000;
        public const int DefaultAcSavingModeTime = 5;
        public const int DefaultAcSavingModeBrightness = 0;
        public const int DefaultBatterySavingModeTime = 3;
        public const int DefaultBatterySavingModeBrightness = 0;
        public const int DefaultTurnOffMonitorWay = 1;
        public const int DefaultTrayIconId = 1;
        public const bool DefaultTrunOffMonitorByThinkVantage = true;

        private static readonly ConfigManager ConfigManager;

        /// <summary>
        /// Gets or sets the current brightness
        /// </summary>
        public static int Brightness { get; private set; }


        /// <summary>
        /// Gets or sets if the app will start automaticlly.
        /// </summary>
        public static bool AutoStart { get; }


        /// <summary>
        /// Gets the OSD Style, 0 is notmal style, 1 is circula style.
        /// </summary>
        public static int OsdStyle { get; }

        /// <summary>
        /// Gets the OSD window show timeout(unit is second).
        /// </summary>
        public static int OsdTimeout { get; }

        /// <summary>
        /// Gets the time of enter AC saving mode(unit is ms).
        /// </summary>
        public static int AcSavingModeTime { get; }

        /// <summary>
        /// Gets the brightness under the AC saving mode.
        /// </summary>
        public static int AcSavingModeBrightness { get; }


        /// <summary>
        /// Gets the time of enter battery saving mode(unit is ms).
        /// </summary>
        public static int BatterySavingModeTime { get; }

        /// <summary>
        /// Gets the brightness under the battery saving mode.
        /// </summary>
        public static int BatterySavingModeBrightness { get; }

        /// <summary>
        /// Gets the TurnOffMonitor way from setting file.
        /// </summary>
        public static TrunOffMonitorWay TrunOffMonitorWay { get; }

        /// <summary>
        /// Gets the trayicon's id, if is zero, trayicon will be disabled.
        /// </summary>
        public static int TrayIconId { get; }

        /// <summary>
        /// Get if TurnOff the monitor when press ThinkVantage key.
        /// </summary>
        public static bool TrunOffMonitorByThinkVantage { get; }


        static SettingManager()
        {
            var settingContent = File.ReadAllText(SettingFilePath);
            ConfigManager = new ConfigManager(settingContent);
            Brightness = ConfigManager.GetValue("Setting","Brightness", DefaultBrightness);
            AutoStart = ConfigManager.GetValue("Setting","AutoStart", DefaultAutoStart);
            OsdStyle = ConfigManager.GetValue("Setting", "OSDStyle", DefaultOsdStyle);
            OsdTimeout = ConfigManager.GetValue("Setting", "OSDTimeOut", DefaultOsdTimeout);
            AcSavingModeTime = ConfigManager.GetValue("Setting", "AcSavingModeTime", DefaultAcSavingModeTime);
            AcSavingModeBrightness = ConfigManager.GetValue("Setting", "AcSavingModeBrightness", DefaultAcSavingModeBrightness);
            BatterySavingModeTime = ConfigManager.GetValue("Setting", "BatterySavingModeTime", DefaultBatterySavingModeTime);
            BatterySavingModeBrightness = ConfigManager.GetValue("Setting", "BatterySavingModeBrightness", DefaultBatterySavingModeBrightness);
            TrunOffMonitorWay = ConfigManager.GetValue("Setting", "TrunOffMonitorWay", DefaultTurnOffMonitorWay) == 0? TrunOffMonitorWay.ZeroBrightness: TrunOffMonitorWay.TurnOff;
            TrayIconId = ConfigManager.GetValue("Setting", "TrayIconId", DefaultTrayIconId);
            TrunOffMonitorByThinkVantage = ConfigManager.GetValue("Setting", "TrunOffMonitorByThinkVantage", DefaultTrunOffMonitorByThinkVantage);
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
    }
}
