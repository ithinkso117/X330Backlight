namespace X330Backlight.Utils.NotifyIcon.Interop
{
    /// <summary>
    /// Main operations performed on the
    /// <see cref="WinApi.Shell_NotifyIcon"/> function.
    /// </summary>
    public enum NotifyCommand
    {
        /// <summary>
        /// The taskbar icon is being created.
        /// </summary>
        Add = 0x00,

        /// <summary>
        /// The settings of the taskbar icon are being updated.
        /// </summary>
        Modify = 0x01,

        /// <summary>
        /// The taskbar icon is deleted.
        /// </summary>
        Delete = 0x02
    }
}
