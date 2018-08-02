using System;
using System.ComponentModel;

namespace X330Backlight.Utils.NotifyIcon.Interop
{
    /// <summary>
    /// Receives messages from the taskbar icon through
    /// window messages of an underlying helper window.
    /// </summary>
    public class WindowMessageSink : IDisposable
    {
        #region members

        /// <summary>
        /// The ID of messages that are received from the the
        /// taskbar icon.
        /// </summary>
        public const int CallbackMessageId = 0x400;

        /// <summary>
        /// A delegate that processes messages of the hidden
        /// native window that receives window messages. Storing
        /// this reference makes sure we don't loose our reference
        /// to the message window.
        /// </summary>
        private WindowProcedureHandler _messageHandler;

        /// <summary>
        /// Window class ID.
        /// </summary>
        private string _windowId;

        /// <summary>
        /// Handle for the message window.
        /// </summary> 
        internal IntPtr MessageWindowHandle { get; private set; }

        /// <summary>
        /// The version of the underlying icon. Defines how
        /// incoming messages are interpreted.
        /// </summary>
        public NotifyIconVersion Version { get; set; }

        #endregion

        #region events

        /// <summary>
        /// The custom tooltip should be closed or hidden.
        /// </summary>
        public event Action<bool> ChangeToolTipStateRequest;

        /// <summary>
        /// Fired in case the user clicked or moved within
        /// the taskbar icon area.
        /// </summary>
        public event Action<MouseEvent> MouseEventReceived;


        #endregion

        #region construction

        /// <summary>
        /// Creates a new message sink that receives message from
        /// a given taskbar icon.
        /// </summary>
        /// <param name="version"></param>
        public WindowMessageSink(NotifyIconVersion version)
        {
            Version = version;
            CreateMessageWindow();
        }


        private WindowMessageSink()
        {
        }


        /// <summary>
        /// Creates a dummy instance that provides an empty
        /// pointer rather than a real window handler.<br/>
        /// Used at design time.
        /// </summary>
        /// <returns></returns>
        internal static WindowMessageSink CreateEmpty()
        {
            return new WindowMessageSink
            {
                MessageWindowHandle = IntPtr.Zero,
                Version = NotifyIconVersion.Vista
            };
        }

        #endregion

        #region CreateMessageWindow

        /// <summary>
        /// Creates the helper message window that is used
        /// to receive messages from the taskbar icon.
        /// </summary>
        private void CreateMessageWindow()
        {
            //generate a unique ID for the window
            _windowId = "WPFTaskbarIcon_" + DateTime.Now.Ticks;

            //register window message handler
            _messageHandler = OnWindowMessageReceived;

            // Create a simple window class which is reference through
            //the messageHandler delegate
            WindowClass wc;

            wc.style = 0;
            wc.lpfnWndProc = _messageHandler;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = "";
            wc.lpszClassName = _windowId;

            // Register the window class
            WinApi.RegisterClass(ref wc);

            // Create the message window
            MessageWindowHandle = WinApi.CreateWindowEx(0, _windowId, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero);

            if (MessageWindowHandle == IntPtr.Zero)
            {
                throw new Win32Exception("Message window handle was not a valid pointer");
            }
        }

        #endregion

        #region Handle Window Messages

        /// <summary>
        /// Callback method that receives messages from the taskbar area.
        /// </summary>
        private IntPtr OnWindowMessageReceived(IntPtr hwnd, uint messageId, IntPtr wparam, IntPtr lparam)
        {
            //forward message
            ProcessWindowMessage(messageId, wparam, lparam);

            // Pass the message to the default window procedure
            return WinApi.DefWindowProc(hwnd, messageId, wparam, lparam);
        }


        /// <summary>
        /// Processes incoming system messages.
        /// </summary>
        /// <param name="msg">Callback ID.</param>
        /// <param name="wParam">If the version is <see cref="NotifyIconVersion.Vista"/>
        /// or higher, this parameter can be used to resolve mouse coordinates.
        /// Currently not in use.</param>
        /// <param name="lParam">Provides information about the event.</param>
        private void ProcessWindowMessage(uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg != CallbackMessageId) return;

            switch (lParam.ToInt32())
            {
                case 0x203:
                    MouseEventReceived(MouseEvent.IconDoubleClick);
                    break;
                case 0x205:
                    MouseEventReceived(MouseEvent.IconRightMouseUp);
                    break;

                case 0x406:
                    if (ChangeToolTipStateRequest != null)
                        ChangeToolTipStateRequest(true);
                    break;

                case 0x407:
                    if (ChangeToolTipStateRequest != null)
                        ChangeToolTipStateRequest(false);
                    break;

                //default:
                //Debug.WriteLine("Unhandled NotifyIcon message ID: " + lParam);
                //    break;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Set to true as soon as <c>Dispose</c> has been invoked.
        /// </summary>
        public bool IsDisposed { get; private set; }


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
        /// This destructor will run only if the <see cref="Dispose()"/>
        /// method does not get called. This gives this base class the
        /// opportunity to finalize.
        /// <para>
        /// Important: Do not provide destructors in types derived from
        /// this class.
        /// </para>
        /// </summary>
        ~WindowMessageSink()
        {
            Dispose(false);
        }


        /// <summary>
        /// Removes the windows hook that receives window
        /// messages and closes the underlying helper window.
        /// </summary>
        private void Dispose(bool disposing)
        {
            //don't do anything if the component is already disposed
            if (IsDisposed) return;
            IsDisposed = true;

            //always destroy the unmanaged handle (even if called from the GC)
            WinApi.DestroyWindow(MessageWindowHandle);
            _messageHandler = null;
        }

        #endregion
    }
}
