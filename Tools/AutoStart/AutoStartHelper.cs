using Microsoft.Win32;

namespace AutoStart
{
    public class AutoStartHelper
    {
        public static void AutoStart(string appName, string exePath, bool autoStart)
        {
            RegistryKey registryKey =
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true) ??
                Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            if (registryKey != null)
            {
                if (autoStart)
                {
                    try
                    {
                        if (registryKey.GetValue(appName) == null)
                        {
                            registryKey.SetValue(appName, exePath);
                        }
                    }
                    catch
                    {
                        //log
                    }
                }
                else
                {
                    try
                    {
                        if (registryKey.GetValue(appName) != null)
                        {
                            registryKey.DeleteValue(appName);
                        }
                    }
                    catch
                    {
                        //log
                    }
                }

                registryKey.Close();
            }
        }
    }
}
