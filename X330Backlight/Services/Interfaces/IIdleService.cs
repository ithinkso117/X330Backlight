using System;

namespace X330Backlight.Services.Interfaces
{
    internal interface IIdleService:IService
    {
        /// <summary>
        /// Raised when enter or exit Idle state. 
        /// </summary>
        event EventHandler IdleStateChanged;

        /// <summary>
        /// Gets if is idle state.
        /// </summary>
        bool IsIdle { get; }
    }
}
