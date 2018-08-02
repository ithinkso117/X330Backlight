using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using X330Backlight.Utils;

namespace X330Backlight
{
    public class BaseWindow:Window
    {
        public BaseWindow()
        {
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.Manual;
            IsHitTestVisible = false;
            Topmost = true;
            Focusable = false;
            SourceInitialized += OnSourceInitialized;
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
            {
                var handle = hwndSource.Handle;
                WinApi.SetWindowLong(handle, WinApi.GwlExstyle, WinApi.GetWindowLong(handle, WinApi.GwlExstyle) | WinApi.WsExNoactivate);
                OnWindowInitialized(hwndSource);
            }
        }

        protected virtual void OnWindowInitialized(HwndSource hwndSource)
        {

        }
    }
}
