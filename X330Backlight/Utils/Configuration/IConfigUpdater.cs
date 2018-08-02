using System.Collections.Generic;
using System.IO;

namespace X330Backlight.Utils.Configuration
{
    public interface IConfigUpdater
    {
        /// <summary>
        /// Set one Item's value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue<T>(string section, string key, T value);

        /// <summary>
        /// Save current config to one stream.
        /// </summary>
        /// <param name="stream"></param>
        void Save(Stream stream);

        /// <summary>
        /// Save the sections to one stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sections"></param>
        void Save(Stream stream, IEnumerable<ConfigSection> sections);
    }
}
