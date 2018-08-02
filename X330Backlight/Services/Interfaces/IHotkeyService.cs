using System;

namespace X330Backlight.Services.Interfaces
{
    internal enum TpHotKey
    {
        BrightnessIncrease,
        BrightnessDecrease,
        ThinkVantage,
    }

    internal interface IHotkeyService : IService
    {
        /// <summary>
        /// Raised when the HotKey(Fn+F8,F9) is pressed.
        /// </summary>
        event EventHandler<TpHotKey> HotKeyTriggered;
    }
}
