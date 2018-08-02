using System;
using System.Runtime.InteropServices;
using System.Text;

namespace X330Backlight.Utils
{
    public class WinApi
    {
        public const int KeyQueryValue = 0x0001;
        public const int KeyNotify = 0x0010;
        public const int StandardRightsRead = 0x00020000;
        public const int WmPowerbroadcast = 0x0218;
        public const int PbtPowersettingchange = 0x8013;
        public const int ServiceControlPowerevent = 0x0000000D;
        public const int GwlExstyle = -20;
        public const int WsExNoactivate = 0x8000000;
        public const int WmSyscommand = 0x0112;
        public const int ScMonitorpower = 0xF170;


        [Flags]
        public enum RegChangeNotifyFilter
        {
            Key = 1,
            Attribute = 2,
            Value = 4,
            Security = 8,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LastinputInfo
        {
            public uint cbSize;
            public uint dwTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PowerbroadcastSetting
        {
            public Guid PowerSetting;

            public uint DataLength;

            public byte Data;
        }

        public enum AcLineStatus : byte
        {
            Offline = 0,
            Online = 1,
            Unknown = 255, // 0xFF
        }

        public enum BatteryFlag : byte
        {
            High = 1,
            Low = 2,
            Critical = 4,
            Charging = 8,
            NoSystemBattery = 128, // 0x80
            Unknown = 255, // 0xFF
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemPowerStatus
        {
            public AcLineStatus LineStatus;
            public BatteryFlag BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
        }


        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegOpenKeyEx(IntPtr hKey, string subKey, uint options, int samDesired, out IntPtr phkResult);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool bWatchSubtree, RegChangeNotifyFilter dwNotifyFilter, IntPtr hEvent, bool fAsynchronous);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(IntPtr hKey);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetLastInputInfo(ref LastinputInfo plii);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern uint GetLastError();

        [DllImport("User32", SetLastError = true)]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid powerSettingGuid, int flags);

        [DllImport("User32", SetLastError = true)]
        public static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemPowerStatus(out SystemPowerStatus sps);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr window, int index);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern long WritePrivateProfileString(string section, string key, string value, string fileName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern long GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder returnValue, uint size, string fileName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
    }
}
