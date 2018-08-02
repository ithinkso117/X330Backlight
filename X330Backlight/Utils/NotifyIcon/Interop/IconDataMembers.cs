using System;

namespace X330Backlight.Utils.NotifyIcon.Interop
{
    /// <summary>
    /// Indicates which members of a <see cref="NotifyIconData"/> structure
    /// were set, and thus contain valid data or provide additional information
    /// to the ToolTip as to how it should display.
    /// </summary>
    [Flags]
    public enum IconDataMembers
    {
        /// <summary>
        /// The message ID is set.
        /// </summary>
        Message = 0x01,

        /// <summary>
        /// The notification icon is set.
        /// </summary>
        Icon = 0x02,

        /// <summary>
        /// The tooltip is set.
        /// </summary>
        Tip = 0x04
    }
}
