using System;
using System.Collections.Generic;
using System.IO;

namespace X330Backlight.Utils.Configuration
{  
    public class ConfigLoader
    {

        /// <summary>
        /// Load sections from one string content.
        /// </summary>
        /// <param name="content">The string content which belong to the config file.</param>
        /// <returns></returns>
        public List<ConfigSection> LoadConfig(string content)
        {
            using (StringReader sr = new StringReader(content))
            {
                return ReadText(sr, (parsedLine) => new ConfigSection(parsedLine));
            }
        }


        /// <summary>
        /// Read all lines from one section.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sr"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        private static List<T> ReadText<T>(TextReader sr, Func<string, T> creator) where T : ConfigSection
        {
            T section = null;
            List<T> sections = new List<T>();

            while (true)
            {
                string line = sr.ReadLine();
                
                if (line == null)
                {
                    if (section != null)
                    {
                        sections.Add(section);
                    }
                    // end
                    break;
                }

                line = line.Trim(' ', '\t');
                if (line.Length == 0 || line[0] == '#')
                {
                    // Ignore comments
                    continue;
                }

                if (!line.StartsWith("["))
                {
                    if (section != null)
                    {
                        string[] items = line.Split(new[] { '=' }, 2);
                        try
                        {
                            string name = items[0].Trim(' ', '\t');
                            string body = items[1].Trim(' ', '\t', '"').Replace("\\r\\n",Environment.NewLine);
                            section.AddItem(name, body);
                        }
                        catch
                        {
                            //Do nothing
                        }
                    }
                }
                else
                {
                    // Close the prev section
                    if (section != null)
                    {
                        sections.Add(section);
                    }

                    // start the new section
                    line = line.Trim('[', ']');

                    section = creator(line);
                }
            }
            return sections;
        }
    }

}
