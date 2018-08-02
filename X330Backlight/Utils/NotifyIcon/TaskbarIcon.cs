using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using X330Backlight.Utils.NotifyIcon.Interop;
using Point = X330Backlight.Utils.NotifyIcon.Interop.Point;

namespace X330Backlight.Utils.NotifyIcon
{
    /// <summary>
    /// A WPF proxy to for a taskbar icon (NotifyIcon) that sits in the system's
    /// taskbar notification area ("system tray").
    /// </summary>
    public partial class TaskbarIcon : FrameworkElement, ITaskbarIcon
    {
        #region Members

        /// <summary>
        /// Represents the current icon data.
        /// </summary>
        private NotifyIconData _iconData;

        /// <summary>
        /// Receives messages from the taskbar icon.
        /// </summary>
        private readonly WindowMessageSink _messageSink;


        private double _scalingFactor = double.NaN;

        /// <summary>
        /// Indicates whether the taskbar icon has been created or not.
        /// </summary>
        private bool _isTaskbarIconCreated;

        #endregion

        #region Construction

        /// <summary>
        /// Inits the taskbar icon and registers a message listener
        /// in order to receive events from the taskbar area.
        /// </summary>
        public TaskbarIcon()
        {
            //using dummy sink in design mode
            _messageSink = Util.IsDesignMode
                ? WindowMessageSink.CreateEmpty()
                : new WindowMessageSink(NotifyIconVersion.Win95);

            //init icon data structure
            _iconData = NotifyIconData.CreateDefault(_messageSink.MessageWindowHandle);

            //create the taskbar icon
            CreateTaskbarIcon();

            //register event listeners
            _messageSink.MouseEventReceived += OnMouseEvent;
            _messageSink.ChangeToolTipStateRequest += OnToolTipChange;

            //register listener in order to get notified when the application closes
            if (Application.Current != null) Application.Current.Exit += OnExit;
        }

        #endregion

        #region Process Incoming Mouse Events

        /// <summary>
        /// Processes mouse events, which are bubbled
        /// through the class' routed events, trigger
        /// certain actions (e.g. show a popup), or
        /// both.
        /// </summary>
        /// <param name="me">Event flag.</param>
        private void OnMouseEvent(MouseEvent me)
        {
            if (_isDisposed) return;

            if (me == MouseEvent.IconDoubleClick)
            {
                DoubleClicked?.Invoke(this, EventArgs.Empty);
            }
            if (!me.IsMatch(MenuActivation)
                || me != MouseEvent.IconRightMouseUp) return;

            //get mouse coordinates
            Point cursorPosition = new Point();
            Interop.WinApi.GetPhysicalCursorPos(ref cursorPosition);

            cursorPosition = GetDeviceCoordinates(cursorPosition);
            ShowContextMenu(cursorPosition);
        }

        #endregion

        #region ToolTips

        /// <summary>
        /// Displays a custom tooltip, if available. This method is only
        /// invoked for Windows Vista and above.
        /// </summary>
        /// <param name="visible">Whether to show or hide the tooltip.</param>
        private void OnToolTipChange(bool visible)
        {
            //if we don't have a tooltip, there's nothing to do here...
            if (TrayToolTipResolved == null) return;

            TrayToolTipResolved.IsOpen = visible;
        }


        private void CreateCustomToolTip()
        {
            //check if the item itself is a tooltip
            ToolTip tt = null;

            if (!String.IsNullOrEmpty(ToolTipText))
            {
                //create a simple tooltip for the ToolTipText string
                tt = new ToolTip();
                tt.Content = ToolTipText;
            }

            //the tooltip explicitly gets the DataContext of this instance.
            //If there is no DataContext, the TaskbarIcon assigns itself
            if (tt != null)
            {
                UpdateDataContext(tt, null, DataContext);
            }

            //store a reference to the used tooltip
            SetTrayToolTipResolved(tt);
        }


        /// <summary>
        /// Sets tooltip settings for the class depending on defined
        /// dependency properties and OS support.
        /// </summary>
        private void WriteToolTipSettings()
        {
            const IconDataMembers flags = IconDataMembers.Tip
                                            | IconDataMembers.Message
                                            | IconDataMembers.Icon;
            _iconData.ToolTipText = ToolTipText;

            if (_messageSink.Version == NotifyIconVersion.Vista)
            {
                //we need to set a tooltip text to get tooltip events from the
                //taskbar icon
                if (String.IsNullOrEmpty(_iconData.ToolTipText) && TrayToolTipResolved != null)
                {
                    //if we have not tooltip text but a custom tooltip, we
                    //need to set a dummy value (we're displaying the ToolTip control, not the string)
                    _iconData.ToolTipText = "ToolTip";
                }
            }

            //update the tooltip text
            Util.WriteIconData(ref _iconData, NotifyCommand.Modify, flags);
        }

        #endregion


        #region Context Menu

        /// <summary>
        /// Displays the <see cref="ContextMenu"/> if
        /// it was set.
        /// </summary>
        private void ShowContextMenu(Point cursorPosition)
        {
            if (_isDisposed) return;

            if (ContextMenu != null)
            {
                //use absolute positioning. We need to set the coordinates, or a delayed opening
                //(e.g. when left-clicked) opens the context menu at the wrong place if the mouse
                //is moved!
                ContextMenu.Placement = PlacementMode.AbsolutePoint;
                ContextMenu.HorizontalOffset = cursorPosition.X;
                ContextMenu.VerticalOffset = cursorPosition.Y;
                ContextMenu.IsOpen = true;

                IntPtr handle = IntPtr.Zero;

                //try to get a handle on the context itself
                HwndSource source = (HwndSource)PresentationSource.FromVisual(ContextMenu);
                if (source != null)
                {
                    handle = source.Handle;
                }

                //if we don't have a handle for the popup, fall back to the message sink
                if (handle == IntPtr.Zero) handle = _messageSink.MessageWindowHandle;

                //activate the context menu or the message window to track deactivation - otherwise, the context menu
                //does not close if the user clicks somewhere else. With the message window
                //fallback, the context menu can't receive keyboard events - should not happen though
                Interop.WinApi.SetForegroundWindow(handle);
            }
        }

        #endregion


        #region Create / Remove Taskbar Icon


        /// <summary>
        /// Creates the taskbar icon. This message is invoked during initialization,
        /// if the taskbar is restarted, and whenever the icon is displayed.
        /// </summary>
        private void CreateTaskbarIcon()
        {
            lock (this)
            {
                if (!_isTaskbarIconCreated)
                {
                    const IconDataMembers members = IconDataMembers.Message
                                                    | IconDataMembers.Icon
                                                    | IconDataMembers.Tip;

                    //write initial configuration
                    var status = Util.WriteIconData(ref _iconData, NotifyCommand.Add, members);
                    if (!status)
                    {
                        //couldn't create the icon - we can assume this is because explorer is not running (yet!)
                        //-> try a bit later again rather than throwing an exception. Typically, if the windows
                        // shell is being loaded later, this method is being reinvoked from OnTaskbarCreated
                        // (we could also retry after a delay, but that's currently YAGNI)
                        return;
                    }

                    _iconData.VersionOrTimeout = (uint)NotifyIconVersion.Vista;
                    _messageSink.Version = NotifyIconVersion.Vista;

                    _isTaskbarIconCreated = true;
                }
            }
        }

        /// <summary>
        /// Closes the taskbar icon if required.
        /// </summary>
        private void RemoveTaskbarIcon()
        {
            lock (this)
            {
                //make sure we didn't schedule a creation

                if (_isTaskbarIconCreated)
                {
                    Util.WriteIconData(ref _iconData, NotifyCommand.Delete, IconDataMembers.Message);
                    _isTaskbarIconCreated = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// Recalculates OS coordinates in order to support WPFs coordinate
        /// system if OS scaling (DPIs) is not 100%.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point GetDeviceCoordinates(Point point)
        {
            if (double.IsNaN(_scalingFactor))
            {
                //calculate scaling factor in order to support non-standard DPIs
                var presentationSource = PresentationSource.FromVisual(this);
                if (presentationSource == null)
                {
                    _scalingFactor = 1;
                }
                else
                {
                    if (presentationSource.CompositionTarget != null)
                    {
                        var transform = presentationSource.CompositionTarget.TransformToDevice;
                        _scalingFactor = 1 / transform.M11;
                    }
                }
            }

            //on standard DPI settings, just return the point
            if ((int)_scalingFactor == 1) return point;

            return new Point() { X = (int)(point.X * _scalingFactor), Y = (int)(point.Y * _scalingFactor) };
        }

        #region Dispose / Exit

        /// <summary>
        /// Set to true as soon as <c>Dispose</c> has been invoked.
        /// </summary>
        private bool _isDisposed;


        /// <summary>
        /// Disposes the class if the application exits.
        /// </summary>
        private void OnExit(object sender, EventArgs e)
        {
            Dispose();
        }


        /// <summary>
        /// This destructor will run only if the <see cref="Dispose()"/>
        /// method does not get called. This gives this base class the
        /// opportunity to finalize.
        /// <para>
        /// Important: Do not provide destructors in types derived from
        /// this class.
        /// </para>
        /// </summary>
        ~TaskbarIcon()
        {
            Dispose(false);
        }


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <remarks>This method is not virtual by design. Derived classes
        /// should override <see cref="Dispose(bool)"/>.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Closes the tray and releases all resources.
        /// </summary>
        /// <summary>
        /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
        /// If disposing equals <c>true</c>, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// </summary>
        /// <param name="disposing">If disposing equals <c>false</c>, the method
        /// has been called by the runtime from inside the finalizer and you
        /// should not reference other objects. Only unmanaged resources can
        /// be disposed.</param>
        /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
        /// the method has already been called.</remarks>
        private void Dispose(bool disposing)
        {
            //don't do anything if the component is already disposed
            if (_isDisposed || !disposing) return;

            lock (this)
            {
                _isDisposed = true;

                //deregister application event listener
                if (Application.Current != null)
                {
                    Application.Current.Exit -= OnExit;
                }

                //stop timers
                //singleClickTimer.Dispose();
                //balloonCloseTimer.Dispose();

                //dispose message sink
                _messageSink.Dispose();

                //remove icon
                RemoveTaskbarIcon();
            }
        }

        #endregion
    }
}
