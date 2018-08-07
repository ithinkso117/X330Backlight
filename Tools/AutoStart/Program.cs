using System;
using System.IO;

namespace AutoStart
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                var name = string.Empty;
                var file = string.Empty;
                var auto = -1;
                foreach (var s in args)
                {
                    var keyAndValue = s.Split('=');
                    if (keyAndValue.Length == 2)
                    {
                        if (keyAndValue[0].ToLower() == "/name")
                        {
                            name = keyAndValue[1];
                        }
                        if (keyAndValue[0].ToLower() == "/file")
                        {
                            file = keyAndValue[1];
                        }
                        if (keyAndValue[0].ToLower() == "/auto")
                        {
                            int.TryParse(keyAndValue[1],out auto);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(name) && File.Exists(file) && (auto == 0 || auto == 1))
                {
                    try
                    {
                        AutoStartHelper.AutoStart(name, file, auto == 1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }
    }
}
