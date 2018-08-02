using X330Backlight.Utils;

namespace X330Backlight.Settings
{
    internal class TimeViewModel
    {
        public int Time { get; }

        public TimeViewModel(int time)
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
