using System;
using System.Windows.Interop;

namespace X330Backlight
{
    internal interface IServiceOwner
    {
        /// <summary>
        /// Gets the HwndSource of the owner.
        /// </summary>
        HwndSource Source { get; }
    }
}
