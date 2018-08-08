using System;

namespace X330Backlight.Services.Interfaces
{
    internal enum LidSwitchStatus
    {
        /// <summary>
        /// The lid switch is in opened status.
        /// </summary>
        Opened,

        /// <summary>
        /// The lid switch is in closed status.
        /// </summary>
        Closed
    }

    internal enum PowerChangeStatus
    {
        /// <summary>
        /// System is about to suspend.
        /// </summary>
        Suspending,

        /// <summary>
        /// System is about to resume.
        /// </summary>
        Resuming,
    }

    internal enum BatteryStatus
    {
        /// <summary>
        /// The battery capacity is at more than 66 percent
        /// </summary>
        High,

        /// <summary>
        /// The battery capacity is at less than 33 percent
        /// </summary>
        Low,

        /// <summary>
        /// The battery capacity is at less than five percent
        /// </summary>
        Critical
    }


    internal class LidSwitchStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the LidSwitch status.
        /// </summary>
        public LidSwitchStatus Status { get; }

        public LidSwitchStatusChangedEventArgs(LidSwitchStatus status)
        {
            Status = status;
        }
    }


    internal class PowerChangeStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the PowerChange status.
        /// </summary>
       public PowerChangeStatus Status { get; }

        public PowerChangeStatusEventArgs(PowerChangeStatus status)
        {
            Status = status;
        }

    }


    internal interface IPowerService : IService
    {
        /// <summary>
        /// Raised when the lid switch status changed.
        /// </summary>
        event EventHandler<LidSwitchStatusChangedEventArgs> LidSwitchStatusChanged;


        /// <summary>
        /// Raised when system is about to resume or suspend.
        /// </summary>
        event EventHandler<PowerChangeStatusEventArgs> PowerStatusChanged;


        /// <summary>
        /// Gets if the Ac power is plgged in.
        /// </summary>
        bool AcPowerPluggedIn { get; }

        /// <summary>
        /// Gets the battery status if AcPowerPluggedin is false.
        /// </summary>
        BatteryStatus BatteryStatus { get; }
    }
}
