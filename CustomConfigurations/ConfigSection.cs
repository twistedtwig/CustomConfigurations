using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomConfigurations
{
    public class ConfigSection
    {
        private ConfigurationGroupElement ConfigElement;
        private IDictionary<string, string> valuesAsDictionary = new Dictionary<string, string>();
        private ConfigSection ParentElement;

        internal ConfigSection(ConfigurationGroupElement configElement) : this(configElement, null) { }

        internal ConfigSection(ConfigurationGroupElement configElement, ConfigSection parent)
        {
            ConfigElement = configElement;
            foreach (ValueItemElement element in configElement.ValueItemCollection)
            {
                //mirroring the behavour of the configurationGroupElement stuff that will just return the last item even if dups keys used.
                if (!valuesAsDictionary.ContainsKey(element.Key))
                {
                    valuesAsDictionary.Add(element.Key, element.Value);
                }
            }

            ParentElement = parent;
        }

        /// <summary>
        /// Returns the name attribute of this config group section.
        /// </summary>
        public string Name
        {
            get { return ConfigElement.Name; }
        }

        public int Index { get { return ConfigElement.Index; } }

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
            set
            {
                if (!ContainsKey(key))
                {
                    ConfigElement.ValueItemCollection.Add(new ValueItemElement { Key = key, Value = value });
                }
                else
                {
                    ConfigElement.ValueItemCollection[key].Value = value;
                }
            }
        }

        public int GetValueItemIndex(string key)
        {
            if (!ContainsKey(key))
            {
                return -1;
            }

            return ConfigElement.ValueItemCollection[key].Index;
        }

        public void Remove(string key)
        {
            ConfigElement.ValueItemCollection.Remove(key);
        }

        /// <summary>
        /// Returns all the values as a dictionary.
        /// </summary>
        public IDictionary<string, string> ValuesAsDictionary
        {
            get { return valuesAsDictionary; }
        }

        /// <summary>
        /// determines if the collection had any inner 'Collections' defined in the configuration.
        /// </summary>
        public bool ContainsSubCollections
        {
            get { return ConfigElement.InnerCollections != null && ConfigElement.InnerCollections.Count > 0; }
        }

        private CollectionsGroup collections;
        /// <summary>
        /// returns the inner collections if there are any, otherwise returns null.
        /// </summary>
        public CollectionsGroup Collections
        {
            get
            {
                if (collections == null && ContainsSubCollections)
                {
                    collections = new CollectionsGroup(ConfigElement.InnerCollections, this);
                }

                return collections;
            }
        }







        /// <summary>
        /// returns the parent object in the collection, will return null at the top level, (ConfigurationGroup).
        /// </summary>
        public ConfigSection Parent
        {
            get { return ParentElement; }
        }

        /// <summary>
        /// Determines if this ConfigSection is a child of another section.
        /// </summary>
        public bool IsChild { get { return Parent != null; } }

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

        /// <summary>
        /// Create the given object and assign values from ValueItems where key matches name exactly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>()
        {
            return Create<T>(false);
        }

        public T Create<T>(bool includePrivateorProtectedProperties)
        {
            if (!valuesAsDictionary.ContainsKey("Name") && !valuesAsDictionary.ContainsKey("name"))
            {
                valuesAsDictionary.Add("Name", Name);
            }

            return ObjectCreator.Create<T>(includePrivateorProtectedProperties, valuesAsDictionary);
        }
    }
}
