using X330Backlight.Utils;

namespace X330Backlight.Settings
{
    internal class OsdTimeoutViewModel
    {
        public int Time { get; }

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
