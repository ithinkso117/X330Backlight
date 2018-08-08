using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using X330Backlight.Services;
using X330Backlight.Services.Interfaces;
using X330Backlight.Utils;

namespace X330Backlight
{
    public class OsdWindow : BaseWindow
    {
        private readonly object _osdLocker = new object();
        private readonly DispatcherTimer _osdDisplayTimer;

        private int _displayCount;

        private ProgressBar _brightnessLevel;
        private TextBlock _brightnessValue;

        public OsdWindow()
        {
            var backlightService = ServiceManager.GetService<IBacklightService>();
            backlightService.BrightnessChanged += OnBrightnessChanged;
            UpdateOsd(backlightService.Brightness);

            _osdDisplayTimer = new DispatcherTimer(DispatcherPriority.Normal) {Interval = TimeSpan.FromSeconds(1)};
            _osdDisplayTimer.Tick += OsdDisplayTimerOnTick;
            _osdDisplayTimer.Start();
        }


        private void OnBrightnessChanged(object sender, EventArgs e)
        {
            var backlightService = (BacklightService) sender;
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                UpdateOsd(backlightService.Brightness);
                Dispatcher.Invoke(DispatcherPriority.Normal,new Action(ShowOsd));
            }));
        }

        private void UpdateOsd(int brightness)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,new Action(() =>
            {
                if (_brightnessLevel == null)
                {
                    _brightnessLevel = (ProgressBar) FindName("BrightnessLevel");
                }

                if (_brightnessValue == null)
                {
                    _brightnessValue = (TextBlock) FindName("BrightnessValue");
                }

                if (_brightnessLevel != null)
                {
                    _brightnessLevel.Value = brightness;
                }

                if (_brightnessValue != null)
                {
                    if (brightness >= 10)
                    {
                        _brightnessValue.Text = brightness.ToString();
                    }
                    else
                    {
                        _brightnessValue.Text =  "  " + brightness;
                    }
                    
                    
                }
            }));
        }


        private void OsdDisplayTimerOnTick(object sender, EventArgs e)
        {
            if (_displayCount == 0)
            {
                if (Visibility == Visibility.Visible)
                {
                    HideOsd();
                }
            }
            else
            {
                lock (_osdLocker)
                {
                    _displayCount--;
                }
            }
        }


        private void HideOsd()
        {
            lock (_osdLocker)
            {
                Hide();
            }
        }

        private void ShowOsd()
        {
            lock (_osdLocker)
            {
                _displayCount = SettingManager.OsdTimeout;
                Visibility = Visibility.Visible;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = SystemParameters.PrimaryScreenHeight - 86 - Height;
        }

        protected override void OnClosed(EventArgs e)
        {
            var backlightService = ServiceManager.GetService<IBacklightService>();
            backlightService.BrightnessChanged -= OnBrightnessChanged;
            _osdDisplayTimer.Stop();
            HideOsd();
            base.OnClosed(e);
        }
    }
}
