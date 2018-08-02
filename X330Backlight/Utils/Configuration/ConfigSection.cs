using System.Collections.Generic;

namespace X330Backlight.Utils.Configuration
{
    public class ConfigSection
    {
        /// <summary>
        /// Gets the secion name of the section.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets all items from the readed file.
        /// </summary>
        public List<KeyValuePair<string, string>> Items { get; }

        public ConfigSection(string name)
        {
            Items = new List<KeyValuePair<string, string>>();
            Name = name;
        }

        /// <summary>
        /// Add one item into the section
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void AddItem(string name, string value)
        {
            int index = Items.FindIndex((pair) => pair.Key == name);
            if (index >= 0)
            {
                Items.RemoveAt(index);
            }
            value = TokenHelper.DeleteComment(value).Trim(' ');
            Items.Add(new KeyValuePair<string, string>(name, value));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
