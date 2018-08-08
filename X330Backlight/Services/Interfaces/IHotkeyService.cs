using System;

namespace X330Backlight.Services.Interfaces
{
    internal enum TpHotkey
    {
        BrightnessIncrease,
        BrightnessDecrease,
        ThinkVantage,
    }

    internal class HotkeyEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the ThinkPad's hotkey.
        /// </summary>
        public TpHotkey Key { get; }

        public HotkeyEventArgs(TpHotkey key)
        {
            Key = key;
        }
    }

    internal interface IHotkeyService : IService
    {
        /// <summary>
        /// Raised when the HotKey(Fn+F8,F9) is pressed.
        /// </summary>
        event EventHandler<HotkeyEventArgs> HotKeyTriggered;
    }
}
