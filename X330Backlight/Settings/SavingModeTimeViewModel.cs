using X330Backlight.Utils;

namespace X330Backlight.Settings
{
    internal class SavingModeTimeViewModel
    {
        /// <summary>
        /// Gets the time of entering SavingModeTime the unit is millisecond
        /// </summary>
        public int Time { get; }

        /// <summary>
        /// Create the SavingModeTimeViewModel
        /// </summary>
        /// <param name="time">The time entering saving mode, unit is millisecond</param>
        public SavingModeTimeViewModel(int time)
        {
            Time = time;
        }

        public override string ToString()
        {
            if (Time == 0)
            {
                return TranslateHelper.Translate("Never");
            }
            if(Time <= 3600000)
            {
                return Time / 60000 + " " + TranslateHelper.Translate("Minute");
            }

            return Time/ 3600000 + " " + TranslateHelper.Translate("Hour");
        }
    }
}
