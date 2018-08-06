using System;
using System.Windows.Input;

namespace X330Backlight.TaskbarIcons
{
    internal class TaskbarIconMenuCommand : ICommand
    {
        private readonly Action _action;
        private bool _isEnbabled;


        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets if menu is enable.
        /// </summary>
        public bool IsEnbabled
        {
            get => _isEnbabled;
            set
            {
                if (_isEnbabled != value)
                {
                    _isEnbabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raised when check can execute.
        /// </summary>
        public TaskbarIconMenuCommand(Action action)
        {
            IsEnbabled = true;
            _action = action;
        }


        public bool CanExecute(object parameter)
        {
            return _isEnbabled;
        }

        /// <summary>
        /// Run the command.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (CanExecute(null))
            {
                _action?.Invoke();
            }
        }

    }
}
