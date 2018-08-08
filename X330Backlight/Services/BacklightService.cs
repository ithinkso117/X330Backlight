using System;
using System.Threading;
using X330Backlight.Services.Interfaces;
using X330Backlight.Utils;

namespace X330Backlight.Services
{
    internal class BacklightService: ServiceBase, IBacklightService
    {

        private const ushort Vid = 4292;
        private const ushort Pid = 33742;

        private readonly object _brightnessLocker = new object();

        private bool _forceLoadBrightness = false;

        private uint _deviceId;
        private int _brightness;

        private readonly ManualResetEvent _eventTerminate = new ManualResetEvent(false);
        private Thread _monitorThread;



        /// <summary>
        /// Raised when the brightness changed.
        /// </summary>
        public event EventHandler BrightnessChanged;


        /// <summary>
        /// Gets if the Backlight is in closed status.
        /// </summary>
        public bool BacklightClosed { get; private set; }


        /// <summary>
        /// Gets or sets the brightness value.
        /// </summary>
        public int Brightness
        {
            get => _brightness;
            set
            {
                if (_brightness != value || _forceLoadBrightness)
                {
                    if (value > 15) value = 15;
                    if (value < 0) value = 0;
                    if (SetBrightness(value))
                    {
                        _brightness = value;
                        BrightnessChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }


        /// <summary>
        /// Save current brightness to the file in the save folder.
        /// </summary>
        public void SaveBrightness()
        {
            SettingManager.SaveBrightness(Brightness);
        }

        /// <summary>
        /// Load the saved brightness from the saved file.
        /// </summary>
        public byte LoadBrightness()
        {
            return (byte)SettingManager.Brightness;
        }


        /// <summary>
        /// Open the backlight
        /// </summary>
        public void TurnOnBacklight()
        {
            if (SettingManager.TurnOffMonitorWay == TurnOffMonitorWay.ZeroBrightness)
            {
                var brightness = LoadBrightness();
                Brightness = brightness;
                BacklightClosed = false;
            }
            else if(SettingManager.TurnOffMonitorWay == TurnOffMonitorWay.TurnOff)
            {
                //TrunOn the monitor
                if (Owner != null)
                {
                    var handle = Owner.Source.Handle;
                    Native.SendMessage(handle, Native.WmSyscommand, Native.ScMonitorpower, -1);
                    BacklightClosed = false;
                }
            }
        }


        /// <summary>
        /// Save current brightness and close the backlight
        /// </summary>
        public void TurnOffBacklight()
        {
            if (SettingManager.TurnOffMonitorWay == TurnOffMonitorWay.ZeroBrightness)
            {
                SaveBrightness();
                Brightness = 0;
                BacklightClosed = true;
            }
            else if(SettingManager.TurnOffMonitorWay == TurnOffMonitorWay.TurnOff)
            {
                //Close the monitor.
                if (Owner != null)
                {
                    var handle = Owner.Source.Handle;
                    Native.SendMessage(handle, Native.WmSyscommand, Native.ScMonitorpower, 2);
                    BacklightClosed = true;
                }
            }
        }


        /// <summary>
        /// Reduce the backlight 
        /// </summary>
        /// <param name="batteryMode">Ture when using battrty</param>
        public void EnterSavingMode(bool batteryMode)
        {
            SaveBrightness();
            Brightness = batteryMode
                ? SettingManager.BatterySavingModeBrightness
                : SettingManager.AcSavingModeBrightness;
        }

        /// <summary>
        /// Exit the saving mode, increase the backlight.
        /// </summary>
        public void ExitSavingMode()
        {
            var brightness = LoadBrightness();
            Brightness = brightness;
        }

        /// <summary>
        /// Set the brightness value to the HID device.
        /// </summary>
        /// <param name="value">The value to send to the HID device</param>
        /// <returns>True success otherwise false.</returns>

        private bool SetBrightness(int value)
        {
            var result = false;
            try
            {
                if (HID.IsOpened(_deviceId) > 0)
                {
                    lock (_brightnessLocker)
                    {
                        var reportBufferLength = HID.GetFeatureReportBufferLength(_deviceId);
                        var buffer = new byte[reportBufferLength];
                        int retryTimes = 3;
                        while (retryTimes > 0)
                        {
                            --retryTimes;
                            buffer[0] = 6;
                            buffer[1] = (byte) (value * 16);
                            if (HID.SetFeatureReport_Control(_deviceId, ref buffer[0], reportBufferLength) == 0 &&
                                HID.GetFeatureReport_Control(_deviceId, ref buffer[0], reportBufferLength) == 0 &&
                                buffer[1] == 1)
                            {
                                retryTimes = 0;
                                result = true;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Write($"Error when set brightness to HID device: {ex}");
            }

            return result;
        }

        /// <summary>
        /// Read the brightness from the HID device.
        /// </summary>
        /// <returns>If the brightness changed, it will return the brightness, otherwise return -1.</returns>
        private int ReadBrightness()
        {
            try
            {
                if (HID.IsOpened(_deviceId) > 0)
                {
                    lock (_brightnessLocker)
                    {
                        var reportBufferLength = HID.GetInputReportBufferLength(_deviceId);
                        var maxReportRequest = HID.GetMaxReportRequest(_deviceId);
                        var bufferSize = reportBufferLength * maxReportRequest;
                        var buffer = new byte[bufferSize];
                        var bytesReturned = 0u;
                        var inputReportInterrupt = HID.GetInputReport_Interrupt(_deviceId, ref buffer[0], bufferSize,
                            maxReportRequest, ref bytesReturned);
                        if (inputReportInterrupt == 0 || inputReportInterrupt == 4)
                        {
                            var brightness = -1;
                            var receivedLength = 0;
                            while (receivedLength < bytesReturned)
                            {
                                if (buffer[receivedLength] == 4)
                                {
                                    brightness = buffer[receivedLength + 1];
                                }

                                receivedLength += reportBufferLength;
                            }

                            if (brightness >= 0)
                            {
                                return brightness;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Write($"Read brightness from HID failed. error:{ex}");
            }
            return -1;
        }


        /// <summary>
        /// Start the service function.
        /// </summary>
        public override void Start()
        {
            if (_deviceId != 0)
            {
                //This is for avoiding duplication start.
                Stop();
            }
            if (HID.GetNumHidDevices(Vid, Pid) > 0)
            {
                var result = HID.Open(ref _deviceId, 0, Vid, Pid, 512);
                if (result == 0)
                {
                    //Set timeout.
                    HID.SetTimeouts(_deviceId, 0, 1000);
                }
            }

            using (new ForceLoadBrightnessTransaction(this))
            {
                //Force Load saved brightness
                Brightness = LoadBrightness();
            }

            _eventTerminate.Reset();
            _monitorThread = new Thread(StartMonitorBrightness);
            _monitorThread.Start();

        }

        /// <summary>
        /// Check the brightness changed, it could be changed by press the power button.
        /// </summary>
        private void StartMonitorBrightness()
        {
            while (!_eventTerminate.WaitOne(200))
            {
                try
                {
                    if (_deviceId != 0)
                    {
                        var brightness = ReadBrightness();
                        if (brightness != -1)
                        {
                            Brightness = brightness;
                            SaveBrightness();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write($"Monitor brightness error:{ex}");
                }
            }
        }


        /// <summary>
        /// Stop the service function.
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
            if (_deviceId != 0)
            {
                //Close the HID
                if (HID.Close(_deviceId) != 0)
                {
                    Logger.Write("Close HID device failed.");
                }

                _deviceId = 0;
            }
        }


        private class ForceLoadBrightnessTransaction : IDisposable
        {
            private readonly BacklightService _backlightService;

            public ForceLoadBrightnessTransaction(BacklightService backlightService)
            {
                _backlightService = backlightService;
                _backlightService._forceLoadBrightness = true;
            }

            public void Dispose()
            {
                _backlightService._forceLoadBrightness = false;
            }
        }
    }
}
