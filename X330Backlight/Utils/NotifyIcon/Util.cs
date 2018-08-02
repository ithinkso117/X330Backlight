using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Resources;
using X330Backlight.Utils.NotifyIcon.Interop;

namespace X330Backlight.Utils.NotifyIcon
{
    /// <summary>
    /// Util and extension methods.
    /// </summary>
    internal static class Util
    {
        private static readonly object SyncRoot = new object();

        #region IsDesignMode

        private static readonly bool _isDesignMode;

        /// <summary>
        /// Checks whether the application is currently in design mode.
        /// </summary>
        public static bool IsDesignMode
        {
            get { return _isDesignMode; }
        }

        #endregion

        #region construction

        static Util()
        {
            _isDesignMode =
                (bool)
                    DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
                        typeof(FrameworkElement))
                        .Metadata.DefaultValue;
        }

        #endregion

        #region WriteIconData

        /// <summary>
        /// Updates the taskbar icons with data provided by a given
        /// <see cref="NotifyIconData"/> instance.
        /// </summary>
        /// <param name="data">Configuration settings for the NotifyIcon.</param>
        /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
        /// <param name="flags">Defines which members of the <paramref name="data"/>
        /// structure are set.</param>
        /// <returns>True if the data was successfully written.</returns>
        /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
        public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command, IconDataMembers flags)
        {
            //do nothing if in design mode
            if (IsDesignMode) return true;

            data.ValidMembers = flags;
            lock (SyncRoot)
            {
                return Interop.WinApi.Shell_NotifyIcon(command, ref data);
            }
        }

        #endregion


        #region ImageSource to Icon

        /// <summary>
        /// Reads a given image resource into a icon.
        /// </summary>
        /// <param name="imageSource">Image source pointing to
        /// an icon file (*.ico).</param>
        /// <returns>An icon object that can be used with the
        /// taskbar area.</returns>
        public static Icon ToIcon(this ImageSource imageSource)
        {
            if (imageSource == null) return null;

            Uri uri = new Uri(imageSource.ToString(), UriKind.RelativeOrAbsolute);
            StreamResourceInfo streamInfo = Application.GetResourceStream(uri);

            if (streamInfo == null)
            {
                string msg = "The supplied image source '{0}' could not be resolved.";
                msg = String.Format(msg, imageSource);
                throw new ArgumentException(msg);
            }

            return new Icon(streamInfo.Stream);
        }

        #endregion

        #region match MouseEvent to PopupActivation

        /// <summary>
        /// Checks if a given <see cref="PopupActivationMode"/> is a match for
        /// an effectively pressed mouse button.
        /// </summary>
        public static bool IsMatch(this MouseEvent me, PopupActivationMode activationMode)
        {
            switch (activationMode)
            {
                case PopupActivationMode.RightClick:
                    return me == MouseEvent.IconRightMouseUp;
                default:
                    return false;
            }
        }

        #endregion

        /// <summary>
        /// Checks whether the <see cref="FrameworkElement.DataContextProperty"/>
        ///  is bound or not.
        /// </summary>
        /// <param name="element">The element to be checked.</param>
        /// <returns>True if the data context property is being managed by a
        /// binding expression.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="element"/>
        /// is a null reference.</exception>
        public static bool IsDataContextDataBound(this FrameworkElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return element.GetBindingExpression(FrameworkElement.DataContextProperty) != null;
        }
    }
}
