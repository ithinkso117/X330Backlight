using X330Backlight.Utils;

namespace X330Backlight.Settings
{
    internal class TurnOffMonitorWayViewModel
    {
        public int Way { get; }

        public TurnOffMonitorWayViewModel(int way)
        {
            Way = way;
        }

        public override string ToString()
        {
            if (Way == 0)
            {
                return TranslateHelper.Translate("ZeroBrightness");
            }

            return TranslateHelper.Translate("TurnOffMonitor");
        }
    }
}
