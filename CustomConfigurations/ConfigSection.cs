using System;
using System.ComponentModel;

namespace CustomConfigurations
{
    public class ConfigSection
    {
        private ConfigurationGroupElement ConfigElement;


        internal ConfigSection(ConfigurationGroupElement configElement)
        {
            ConfigElement = configElement;
        }
        /// <summary>
        /// Returns the number of values found for that config group
        /// </summary>
        public int Count { get { return ConfigElement.ValueItemCollection.Count; } }

        /// <summary>
        /// Checks to see if a given key / value pair is present for the given config group.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return ConfigElement.ValueItemCollection[key] != null;
        }

        public string this[string key]
        {
            get
            {
                if (!ContainsKey(key))
                {
                    return null;
                }

                ValueItemElement item = ConfigElement.ValueItemCollection[key];
                return item != null ? item.Value : null;
            }
        }

        /// <summary>
        /// Tries to parse the value for the given key and return the type converted into the generic Type provided.
        /// 
        /// The OUT Result indicates if the conversion was successful or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public T TryParse<T>(string key, out bool result)
        {
            result = false;
            //check we have real value first.
            string value = this[key];
            if (!ContainsKey(key))
            {
                return default(T);
            }

            T instance = Activator.CreateInstance<T>();
            TypeConverter converter = TypeDescriptor.GetConverter(instance.GetType());
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    object val = converter.ConvertFromInvariantString(value);
                    if (val == null)
                        return default(T);

                    result = true;
                    return (T)val;
                }
                catch (Exception)
                {
                    return default(T);
                }
            }

            return default(T);
        }
    }
}
