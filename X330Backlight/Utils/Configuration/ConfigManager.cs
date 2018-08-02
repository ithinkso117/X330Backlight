using System;
using System.Collections.Generic;
using System.IO;

namespace X330Backlight.Utils.Configuration
{
    public class ConfigManager : IConfigUpdater
    {
        private readonly Dictionary<string, Dictionary<string, string>> _sections =
            new Dictionary<string, Dictionary<string, string>>();

        public ConfigManager()
        {

        }

        /// <summary>
        /// Construct the ConfigManager by given content.
        /// </summary>
        /// <param name="content"></param>
        public ConfigManager(string content)
        {
            var sections = LoadSections(content);
            foreach (var section in sections)
            {
                _sections[section.Name] = new Dictionary<string, string>();

                foreach (var parameter in section.Items)
                {
                    _sections[section.Name][parameter.Key] = parameter.Value;
                }
            }
        }

        /// <summary>
        /// Load sections from config content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public IEnumerable<ConfigSection> GetSections(string content)
        {
            return LoadSections(content);
        }

        /// <summary>
        /// Load all sections from context.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private List<ConfigSection> LoadSections(string content)
        {
            var configLoader = new ConfigLoader();
            return configLoader.LoadConfig(content);
        }


        /// <summary>
        /// Get one item value of one section.
        /// </summary>
        /// <typeparam name="T">The type of return value.</typeparam>
        /// <param name="section">the section's name</param>
        /// <param name="key">The key's name</param>
        /// <param name="defaultValue">The default value of the T</param>
        /// <returns></returns>
        public T GetValue<T>(string section, string key, T defaultValue)
        {
            Type t = typeof(T);
            if (_sections.TryGetValue(section, out var sectionContent) &&
                sectionContent.TryGetValue(key, out var value))
            {
                if (t == typeof(string))
                {
                    return (T) ((object) value);
                }
                if (t == typeof(bool) && (value == "0" || value == "1"))
                {
                    return (T) ((object) (value != "0"));
                }
                return (T) Convert.ChangeType(value, t);
            }
            return defaultValue;
        }

        /// <summary>
        /// Set one Item's value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void IConfigUpdater.SetValue<T>(string section, string key, T value)
        {
            if (!_sections.TryGetValue(section, out var sectionContent))
            {
                sectionContent = new Dictionary<string, string>();
                _sections.Add(section, sectionContent);
            }
            if (!sectionContent.TryGetValue(key, out _))
            {
                sectionContent.Add(key, null);
            }
            sectionContent[key] = value?.ToString();
        }

        /// <summary>
        /// Save sections into one stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sections"></param>
        void IConfigUpdater.Save(Stream stream, IEnumerable<ConfigSection> sections)
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                foreach (var sectionContent in sections)
                {
                    sw.WriteLine($"[{sectionContent.Name}]");
                    foreach (var item in sectionContent.Items)
                    {
                        sw.WriteLine($"{item.Key} = {item.Value}");
                    }
                }
                sw.Flush();
            }
        }

        /// <summary>
        /// Save current config into one stream.
        /// </summary>
        /// <param name="stream"></param>
        void IConfigUpdater.Save(Stream stream)
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                foreach (var sectionContent in _sections)
                {
                    sw.WriteLine($"[{sectionContent.Key}]");
                    foreach (var item in sectionContent.Value)
                    {
                        sw.WriteLine($"{item.Key} = {item.Value}");
                    }
                }
                sw.Flush();
            }
        }
    }
}