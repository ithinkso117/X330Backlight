using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using X330Backlight.Services.Interfaces;
using X330Backlight.Utils;

namespace X330Backlight.Services
{

    internal class PowerService : ServiceBase, IPowerService
    {
        private Guid _guidLidswitchStateChange = new Guid(0xBA3E0F4D, 0xB817, 0x4094, 0xA2, 0xD1, 0xD5, 0x63, 0x79, 0xE6, 0xA0, 0xF3);

        private IntPtr _lidSwitchChangeHandle;

        /// <summary>
        /// Raised when the lid switch status changed.
        /// </summary>
        public event EventHandler<LidSwitchStatus> LidSwitchStatusChanged;


        /// <summary>
        /// Raised when system is about to resume.
        /// </summary>
        public event EventHandler<PowerChangeStatus> PowerChangeStatusChanged;



        /// <summary>
        /// Gets if the Ac power is plgged in.
        /// </summary>
        public bool AcPowerPluggedIn
        {
            get
            {
                WinApi.GetSystemPowerStatus(out var sps);
                return sps.LineStatus == WinApi.AcLineStatus.Online;
            }
        }

        public PowerService()
        {
            //Always monitor the powerMode changed event. start and stop will not affect it.
            SystemEvents.PowerModeChanged += PowerModeChanged;
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == WinApi.WmPowerbroadcast)
            {
                if (wparam.ToInt32() == WinApi.PbtPowersettingchange)
                {
                    WinApi.PowerbroadcastSetting setting = (WinApi.PowerbroadcastSetting)Marshal.PtrToStructure(lparam, typeof(WinApi.PowerbroadcastSetting));
                    if (setting.PowerSetting == _guidLidswitchStateChange)
                    {
                        LidSwitchStatusChanged?.Invoke(this, setting.Data == 1? LidSwitchStatus.Opened: LidSwitchStatus.Closed);
                    }
                }
            }
            return IntPtr.Zero;
        }

        private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                PowerChangeStatusChanged?.Invoke(this, PowerChangeStatus.Resuming);
            }
            else if (e.Mode == PowerModes.Suspend)
            {
                PowerChangeStatusChanged?.Invoke(this, PowerChangeStatus.Suspending);
            }
        }


        /// <summary>
        /// Start the function of service.
        /// </summary>
        public override void Start()
        {
            if (_lidSwitchChangeHandle != IntPtr.Zero)
            {
                //For avoiding duplicated start.
                Stop();
            }
            if (Owner != null)
            {
                var handle = Owner.Source.Handle;
                _lidSwitchChangeHandle = WinApi.RegisterPowerSettingNotification(handle, ref _guidLidswitchStateChange, 0);
                if (_lidSwitchChangeHandle == IntPtr.Zero)
                {
                    Logger.Write("Register PowerSettingNotification failed.");
                }
                else
                {
                    Owner.Source.AddHook(WndProc);
                }
            }
        }


        /// <summary>
        /// Stop the function of the service.
        /// </summary>
        public override void Stop()
        {
            if (_lidSwitchChangeHandle != IntPtr.Zero)
            {
                if (!WinApi.UnregisterPowerSettingNotification(_lidSwitchChangeHandle))
                {
                    Logger.Write("UnRegister PowerSettingNotification failed.");
                }
                Owner.Source.RemoveHook(WndProc);
                _lidSwitchChangeHandle = IntPtr.Zero;
            }
        }

        public override void Close()
        {
            base.Close();
            SystemEvents.PowerModeChanged -= PowerModeChanged;
        }
    }
}
