using X330Backlight.Utils;

namespace X330Backlight.Settings
{
    internal class OsdTimeoutViewModel
    {
        /// <summary>
        /// Gets the timeout value of the OSD timeout, the unit is second.
        /// </summary>
        public int Time { get; }

        /// <summary>
        /// Create the OsdTimeoutViewModel
        /// </summary>
        /// <param name="time">The timeout value, unit is second, scope is [1-5]</param>
        public OsdTimeoutViewModel(int time)
        {
            Time = time;
        }

        public override string ToString()
        {
            return Time + " " + TranslateHelper.Translate("Second");
        }
    }
}
