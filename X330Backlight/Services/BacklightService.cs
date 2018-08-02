using System;
using X330Backlight.Services.Interfaces;
using X330Backlight.Utils;

namespace X330Backlight.Services
{
    internal class BacklightService: ServiceBase, IBacklightService
    {

        private const ushort Vid = 4292;
        private const ushort Pid = 33742;

        private uint _deviceId;
        private int _brightness;



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
                if (_brightness != value)
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
            if (SettingManager.TrunOffMonitorWay == TrunOffMonitorWay.ZeroBrightness)
            {
                var brightness = LoadBrightness();
                Brightness = brightness;
                BacklightClosed = false;
            }
            else if(SettingManager.TrunOffMonitorWay == TrunOffMonitorWay.TurnOff)
            {
                //TrunOn the monitor
                if (Owner != null)
                {
                    var handle = Owner.Source.Handle;
                    WinApi.SendMessage(handle, WinApi.WmSyscommand, WinApi.ScMonitorpower, -1);
                    BacklightClosed = false;
                }
            }
        }


        /// <summary>
        /// Save current brightness and close the backlight
        /// </summary>
        public void TrunOffBacklight()
        {
            if (SettingManager.TrunOffMonitorWay == TrunOffMonitorWay.ZeroBrightness)
            {
                SaveBrightness();
                Brightness = 0;
                BacklightClosed = true;
            }
            else if(SettingManager.TrunOffMonitorWay == TrunOffMonitorWay.TurnOff)
            {
                //Close the monitor.
                if (Owner != null)
                {
                    var handle = Owner.Source.Handle;
                    WinApi.SendMessage(handle, WinApi.WmSyscommand, WinApi.ScMonitorpower, 2);
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
                    var reportBufferLength = HID.GetFeatureReportBufferLength(_deviceId);
                    var buffer = new byte[reportBufferLength];
                    int retryTimes = 3;
                    while (retryTimes > 0)
                    {
                        --retryTimes;
                        buffer[0] = 6;
                        buffer[1] = (byte)(value * 16);
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
            catch(Exception ex)
            {
                Logger.Write($"Error when set brightness to HID device: {ex}");
            }

            return result;
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
            //Load saved brightness
            Brightness = LoadBrightness();
        }


        /// <summary>
        /// Stop the service function.
        /// </summary>
        public override void Stop()
        {
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
    }
}
