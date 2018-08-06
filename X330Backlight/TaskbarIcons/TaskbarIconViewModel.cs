using System.Windows.Input;

namespace X330Backlight.TaskbarIcons
{
    internal class TaskbarIconViewModel
    {
        /// <summary>
        /// Gets the setting command.
        /// </summary>
        public ICommand SettingCommand { get; }

        /// <summary>
        /// Gets the exit command.
        /// </summary>
        public ICommand ExitCommand { get; }


        public TaskbarIconViewModel(ICommand settingCommand, ICommand exitCommand)
        {
            SettingCommand = settingCommand;
            ExitCommand = exitCommand;
        }
    }
}
