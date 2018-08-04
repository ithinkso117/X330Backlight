using System;
using System.Runtime.InteropServices;
using System.Threading;
using X330Backlight.Services.Interfaces;
using X330Backlight.Utils;

namespace X330Backlight.Services
{
    internal class IdleService:ServiceBase,IIdleService
    {
        private readonly ManualResetEvent _eventTerminate = new ManualResetEvent(false);

        private readonly byte[] _stateData = new byte[sizeof(ulong)];

        private bool _isIdle;

        private Thread _checkIdleStateThread;

        /// <summary>
        /// Raised when idle state changed.
        /// </summary>
        public event EventHandler IdleStateChanged;

        /// <summary>
        /// Gets if is in idle state.
        /// </summary>
        public bool IsIdle
        {
            get => _isIdle;
            private set
            {
                if (_isIdle != value)
                {
                    _isIdle = value;
                    IdleStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }


        /// <summary>
        /// Get last user input(mouse or keyboard) time in ms.
        /// </summary>
        /// <returns>last user input(mouse or keyboard) time in ms.</returns>
        private int GetLastInputTime()
        {
            var lastinputInfo = new WinApi.LastinputInfo();
            lastinputInfo.cbSize = (uint)Marshal.SizeOf((object)lastinputInfo);
            if (!WinApi.GetLastInputInfo(ref lastinputInfo))
            {
                var error = WinApi.GetLastError().ToString();
                Logger.Write($"Get last input time failed, error:{error}");
            }   
            return (int)lastinputInfo.dwTime;
        }


        /// <summary>
        /// Gets if someone(player) is preventing enter idle.
        /// </summary>
        /// <returns></returns>
        private bool IsPreventingIdle()
        {
            Array.Clear(_stateData,0,_stateData.Length);
            var result = WinApi.CallNtPowerInformation(
                WinApi.SystemExecutionState,
                IntPtr.Zero, 
                0,
                _stateData,
                sizeof(ulong)
            );
            if (result == 0)
            {
                var state = BitConverter.ToUInt64(_stateData, 0);
                if((state & WinApi.EsDisplayRequired) == WinApi.EsDisplayRequired ||
                   (state & WinApi.EsAwaymodeRequired) == WinApi.EsAwaymodeRequired||
                   (state & WinApi.EsSystemRequired) == WinApi.EsSystemRequired)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// The thread proce to check is system enter the idle state.
        /// </summary>
        private void StartCheckIdleState()
        {
            try
            {
                while (!_eventTerminate.WaitOne(1000))
                {
                    SetIdleState();
                }
            }
            catch(Exception ex)
            {
                Logger.Write($"Check Idle state error:{ex}");
            }
        }

        /// <summary>
        /// Calc if enter the idle state, and set the idle state.
        /// </summary>
        private void SetIdleState()
        {
            var powerService = ServiceManager.GetService<IPowerService>();
            var idleTime = SettingManager.AcSavingModeTime;
            if (!powerService.AcPowerPluggedIn)
            {
                idleTime = SettingManager.BatterySavingModeTime;
            }

            if (idleTime > 0)
            {
                var lastInputTime = GetLastInputTime();
                var currentTime = Environment.TickCount;
                var timeElapsed = currentTime - lastInputTime;
                IsIdle = timeElapsed >= idleTime && !IsPreventingIdle();
            }
            else
            {
                IsIdle = false;
            }
        }

        /// <summary>
        /// Start the function of this service.
        /// </summary>
        public override void Start()
        {
            if (_checkIdleStateThread != null)
            {
                Stop();
            }
            SetIdleState();
            _checkIdleStateThread = new Thread(StartCheckIdleState);
            _checkIdleStateThread.Start();
        }


        /// <summary>
        /// Stop the function of this service.
        /// </summary>
        public override void Stop()
        {
            //Close the check thread.
            _eventTerminate.Set();
            if (_checkIdleStateThread != null)
            {
                if (!_checkIdleStateThread.Join(StopTimeout))
                {
                    try
                    {
                        _checkIdleStateThread.Abort();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write($"Stop the idle check thread error:{ex}");
                    }
                }
                _checkIdleStateThread = null;
            }
        }
    }
}
