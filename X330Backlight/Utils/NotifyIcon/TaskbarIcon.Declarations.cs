using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using X330Backlight.Utils.NotifyIcon.Interop;

namespace X330Backlight.Utils.NotifyIcon
{
    /// <summary>
    /// Contains declarations of WPF dependency properties
    /// and events.
    /// </summary>
    partial class TaskbarIcon
    {

        #region TrayToolTipResolved

        /// <summary>
        /// TrayToolTipResolved Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey TrayToolTipResolvedPropertyKey
            = DependencyProperty.RegisterReadOnly("TrayToolTipResolved", typeof(ToolTip), typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));


        /// <summary>
        /// A read-only dependency property that returns the <see cref="ToolTip"/>
        /// that is being displayed.
        /// </summary>
        public static readonly DependencyProperty TrayToolTipResolvedProperty
            = TrayToolTipResolvedPropertyKey.DependencyProperty;


        /// <summary>
        /// Raised when double clicked.
        /// </summary>
        public event EventHandler DoubleClicked;

        /// <summary>
        /// Gets the TrayToolTipResolved property. Returns 
        /// a <see cref="ToolTip"/> control that was created 
        /// <see cref="ToolTipText"/>.
        /// </summary>
        [Browsable(true)]
        [Bindable(true)]
        public ToolTip TrayToolTipResolved
        {
            get { return (ToolTip)GetValue(TrayToolTipResolvedProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the <see cref="TrayToolTipResolved"/>
        /// property.  
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        private void SetTrayToolTipResolved(ToolTip value)
        {
            SetValue(TrayToolTipResolvedPropertyKey, value);
        }

        #endregion

        //DEPENDENCY PROPERTIES

        #region Icon property / IconSource dependency property

        private Icon _icon;

        /// <summary>
        /// Gets or sets the icon to be displayed. This is not a
        /// dependency property - if you want to assign the property
        /// through XAML, please use the <see cref="IconSource"/>
        /// dependency property.
        /// </summary>
        [Browsable(false)]
        public Icon Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                _iconData.IconHandle = value == null ? IntPtr.Zero : _icon.Handle;

                Util.WriteIconData(ref _iconData, NotifyCommand.Modify, IconDataMembers.Icon);
            }
        }


        /// <summary>
        /// Resolves an image source and updates the <see cref="Icon" /> property accordingly.
        /// </summary>
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource",
                typeof(ImageSource),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null, IconSourcePropertyChanged));

        /// <summary>
        /// A property wrapper for the <see cref="IconSourceProperty"/>
        /// dependency property:<br/>
        /// Resolves an image source and updates the <see cref="Icon" /> property accordingly.
        /// </summary>
        [Description("Sets the displayed taskbar icon.")]
        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }


        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="IconSourceProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnIconSourcePropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void IconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskbarIcon owner = (TaskbarIcon)d;
            owner.OnIconSourcePropertyChanged(e);
        }


        /// <summary>
        /// Handles changes of the <see cref="IconSourceProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="IconSource"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnIconSourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ImageSource newValue = (ImageSource)e.NewValue;

            //resolving the ImageSource at design time is unlikely to work
            if (!Util.IsDesignMode) Icon = newValue.ToIcon();
        }

        #endregion

        #region ToolTipText dependency property

        /// <summary>
        /// A tooltip text that is being displayed if no custom <see cref="ToolTip"/>
        /// was set or if custom tooltips are not supported.
        /// </summary>
        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register("ToolTipText",
                typeof(string),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(String.Empty, ToolTipTextPropertyChanged));


        /// <summary>
        /// A property wrapper for the <see cref="ToolTipTextProperty"/>
        /// dependency property:<br/>
        /// A tooltip text that is being displayed if no custom <see cref="ToolTip"/>
        /// was set or if custom tooltips are not supported.
        /// </summary>
        [Description("Alternative to a fully blown ToolTip, which is only displayed on Vista and above.")]
        public string ToolTipText
        {
            get { return (string)GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }


        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="ToolTipTextProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnToolTipTextPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void ToolTipTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskbarIcon owner = (TaskbarIcon)d;
            owner.OnToolTipTextPropertyChanged(e);
        }


        /// <summary>
        /// Handles changes of the <see cref="ToolTipTextProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="ToolTipText"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnToolTipTextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //do not touch tooltips if we have a custom tooltip element
            ToolTip currentToolTip = TrayToolTipResolved;
            if (currentToolTip == null)
            {
                //if we don't have a wrapper tooltip for the tooltip text, create it now
                CreateCustomToolTip();
            }
            else
            {
                //if we have a wrapper tooltip that shows the old tooltip text, just update content
                currentToolTip.Content = e.NewValue;
            }

            WriteToolTipSettings();
        }

        #endregion

        #region MenuActivation dependency property

        /// <summary>
        /// Defines what mouse events display the context menu.
        /// Defaults to <see cref="PopupActivationMode.RightClick"/>.
        /// </summary>
        public static readonly DependencyProperty MenuActivationProperty =
            DependencyProperty.Register("MenuActivation",
                typeof(PopupActivationMode),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(PopupActivationMode.RightClick));

        /// <summary>
        /// A property wrapper for the <see cref="MenuActivationProperty"/>
        /// dependency property:<br/>
        /// Defines what mouse events display the context menu.
        /// Defaults to <see cref="PopupActivationMode.RightClick"/>.
        /// </summary>
        [Description("Defines what mouse events display the context menu.")]
        public PopupActivationMode MenuActivation
        {
            get { return (PopupActivationMode)GetValue(MenuActivationProperty); }
            set { SetValue(MenuActivationProperty, value); }
        }

        #endregion

        #region DataContext dependency property override / target update

        /// <summary>
        /// Updates the <see cref="FrameworkElement.DataContextProperty"/> of a given
        /// <see cref="FrameworkElement"/>. This method only updates target elements
        /// that do not already have a data context of their own, and either assigns
        /// the <see cref="FrameworkElement.DataContext"/> of the NotifyIcon, or the
        /// NotifyIcon itself, if no data context was assigned at all.
        /// </summary>
        private void UpdateDataContext(FrameworkElement target, object oldDataContextValue, object newDataContextValue)
        {
            //if there is no target or it's data context is determined through a binding
            //of its own, keep it
            if (target == null || target.IsDataContextDataBound()) return;

            //if the target's data context is the NotifyIcon's old DataContext or the NotifyIcon itself,
            //update it
            if (ReferenceEquals(this, target.DataContext) || Equals(oldDataContextValue, target.DataContext))
            {
                //assign own data context, if available. If there is no data
                //context at all, assign NotifyIcon itself.
                target.DataContext = newDataContextValue ?? this;
            }
        }

        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="FrameworkElement.DataContextProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnDataContextPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void DataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskbarIcon owner = (TaskbarIcon)d;
            owner.OnDataContextPropertyChanged(e);
        }


        /// <summary>
        /// Handles changes of the <see cref="FrameworkElement.DataContextProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="FrameworkElement.DataContext"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnDataContextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            object newValue = e.NewValue;
            object oldValue = e.OldValue;

            //replace custom data context for ToolTips, Popup, and
            //ContextMenu
            UpdateDataContext(TrayToolTipResolved, oldValue, newValue);
            UpdateDataContext(ContextMenu, oldValue, newValue);
        }

        #endregion

        #region ContextMenu dependency property override

        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="FrameworkElement.ContextMenuProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnContextMenuPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void ContextMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskbarIcon owner = (TaskbarIcon)d;
            owner.OnContextMenuPropertyChanged(e);
        }


        /// <summary>
        /// Releases the old and updates the new <see cref="ContextMenu"/> property
        /// in order to reflect both the NotifyIcon's <see cref="FrameworkElement.DataContext"/>
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnContextMenuPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateDataContext((ContextMenu)e.NewValue, null, DataContext);
        }

        #endregion

        /// <summary>
        /// Registers properties.
        /// </summary>
        static TaskbarIcon()
        {
            //register change listener for the DataContext property
            var md = new FrameworkPropertyMetadata(new PropertyChangedCallback(DataContextPropertyChanged));
            DataContextProperty.OverrideMetadata(typeof(TaskbarIcon), md);

            //register change listener for the ContextMenu property
            md = new FrameworkPropertyMetadata(new PropertyChangedCallback(ContextMenuPropertyChanged));
            ContextMenuProperty.OverrideMetadata(typeof(TaskbarIcon), md);
        }
    }
}
