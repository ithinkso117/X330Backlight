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
        Resuming
    }


    internal interface IPowerService : IService
    {
        /// <summary>
        /// Raised when the lid switch status changed.
        /// </summary>
        event EventHandler<LidSwitchStatus> LidSwitchStatusChanged;


        /// <summary>
        /// Raised when system is about to resume or suspend.
        /// </summary>
        event EventHandler<PowerChangeStatus> PowerChangeStatusChanged;


        /// <summary>
        /// Gets if the Ac power is plgged in.
        /// </summary>
        bool AcPowerPluggedIn { get; }
    }
}
