using System;
using System.Threading;
using Microsoft.Win32;
using X330Backlight.Services.Interfaces;
using X330Backlight.Utils;

namespace X330Backlight.Services
{


    internal class HotkeyService : ServiceBase, IHotkeyService
    {

        private const string NotificationKey = "SYSTEM\\CurrentControlSet\\Services\\IBMPMSVC\\Parameters\\Notification";

        private readonly IntPtr _localMachineKey = new IntPtr(unchecked((int)0x80000002));
        private readonly ManualResetEvent _eventTerminate = new ManualResetEvent(false);


        private RegistryKey _notificationKey;
        private Thread _monitorThread;
        private uint _lastNotificationValue;


        /// <summary>
        /// Raised when the HotKey(Fn+F8,F9) is pressed.
        /// </summary>
        public event EventHandler<TpHotKey> HotKeyTriggered;


        /// <summary>
        /// The hotkey of the backlight is stored in the register.
        /// The lenove's hotkey app must be installed firstly.
        /// We can monitor the registry to know when the user press down the fn+f8 or fn+f9. 
        /// </summary>
        private void StartMonitorRegisterKey()
        {
            try
            {
                var result = Native.RegOpenKeyEx(_localMachineKey, NotificationKey, 0,
                    Native.StandardRightsRead | Native.KeyQueryValue | Native.KeyNotify,
                    out var notifyKey);
                if (result != 0)
                {
                    Logger.Write($"Open registry to monitor failed. Code:{result}");
                    return;
                }

                try
                {
                    const Native.RegChangeNotifyFilter filter =
                        Native.RegChangeNotifyFilter.Key | Native.RegChangeNotifyFilter.Attribute |
                        Native.RegChangeNotifyFilter.Value | Native.RegChangeNotifyFilter.Security;
                    var eventNotify = new AutoResetEvent(false);
                    var eventNotifyHandle = eventNotify.SafeWaitHandle.DangerousGetHandle();
                    WaitHandle[] waitHandles = {eventNotify, _eventTerminate};
                    while (!_eventTerminate.WaitOne(1))
                    {
                        result = Native.RegNotifyChangeKeyValue(notifyKey, true, filter, eventNotifyHandle, true);
                        if (result != 0)
                        {
                            Logger.Write($"Register registry monitor failed. Code:{result}");
                            break;
                        }

                        if (WaitHandle.WaitAny(waitHandles) == 0)
                        {
                            try
                            {
                                if (_notificationKey != null)
                                {
                                    var currentNotificationValue = Convert.ToUInt32(_notificationKey.GetValue(""));
                                    var num = currentNotificationValue ^ _lastNotificationValue;
                                    _lastNotificationValue = currentNotificationValue;
                                    if (num == 32768)
                                    {
                                        HotKeyTriggered?.Invoke(this, TpHotKey.BrightnessIncrease);
                                    }
                                    else if (num == 65536)
                                    {
                                        HotKeyTriggered?.Invoke(this, TpHotKey.BrightnessDecrease);
                                    }
                                    else if (num == 8388608)
                                    {
                                        HotKeyTriggered?.Invoke(this, TpHotKey.ThinkVantage);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Write($"Handle registry changed error:{ex}");
                            }
                        }
                    }
                }
                finally
                {
                    if (notifyKey != IntPtr.Zero)
                    {
                        if (Native.RegCloseKey(notifyKey) != 0)
                        {
                            Logger.Write($"Close the registry monitor failed. Code:{result}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write($"StartMonitorRegisterKey thread error, this may caused by abort a thread. Code:{ex}");
            }
        }


        /// <summary>
        /// Start the function of service.
        /// </summary>
        public override void Start()
        {
            if (_monitorThread != null)
            {   
                //This is for avoiding the duplicate start.
                Stop();
            }
            //Init the increase and reduce value from registry key.
            _notificationKey = Registry.LocalMachine.OpenSubKey(NotificationKey, false);
            if (_notificationKey != null)
            {
                _lastNotificationValue = Convert.ToUInt32(_notificationKey.GetValue(""));
            }

            _eventTerminate.Reset();
            _monitorThread = new Thread(StartMonitorRegisterKey);
            _monitorThread.Start();
        }

        /// <summary>
        /// Stop the function of the service.
        /// </summary>
        public override void Stop()
        {
            //Close the monitor thread.
            _eventTerminate.Set();
            if (_monitorThread != null)
            {
                if (!_monitorThread.Join(StopTimeout))
                {
                    try
                    {
                        _monitorThread.Abort();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write($"Stop the monitor thread error:{ex}");
                    }
                }
                _monitorThread = null;
            }

            //Close registry
            _notificationKey?.Close();
        }
    }
}
