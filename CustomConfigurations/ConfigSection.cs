using System;
using System.Collections.Generic;
using System.ComponentModel;
using CustomConfigurations.ObjectCreation;

namespace CustomConfigurations
{
    public class ConfigSection
    {
        private ConfigurationGroupElement ConfigElement;
        private ConfigValueDictionary valuesAsDictionary = new ConfigValueDictionary();
        private ConfigSection ParentElement;
        private readonly bool AllowValueInheritance;

        internal ConfigSection(ConfigurationGroupElement configElement, bool allowValueInheritance) : this(configElement, null, allowValueInheritance) { }

        internal ConfigSection(ConfigurationGroupElement configElement, ConfigSection parent, bool allowValueInheritance)
        {
            AllowValueInheritance = allowValueInheritance;
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

            //add values from parent
            if (allowValueInheritance && parent != null)
            {
                foreach (ConfigValueItem parentValue in parent.valuesAsDictionary)
                {
                    ValuesAsDictionary.Add(parentValue.Key, parentValue.Value, false, true);
                }
            }
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
        public ConfigValueDictionary ValuesAsDictionary
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
                    collections = new CollectionsGroup(ConfigElement.InnerCollections, this, AllowValueInheritance);
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
            return Create<T>(true);
        }

        public T Create<T>(bool onlySetPublicProperties)
        {
            return Create<T>(null, onlySetPublicProperties);
        }

        public T Create<T>(ConfigValueDictionary mappings)
        {            
            return Create<T>(mappings, true);
        }

        public T Create<T>(ConfigValueDictionary mappings, bool onlySetPublicProperties)
        {
            return Create<T>(CreateCreationSettingsCollection(valuesAsDictionary, mappings, onlySetPublicProperties));
        }

        public T Create<T>(ObjectCreationSettingsCollection mappings)
        {
            if (mappings == null)
                throw new ArgumentException("mappings is null");

            return ObjectCreationAndPopulationFactory.Create<T>(mappings);
        }

        protected ObjectCreationSettingsCollection CreateCreationSettingsCollection(ConfigValueDictionary configValues, ConfigValueDictionary mappings, bool onlySetPublicProperties)
        {
            if (!valuesAsDictionary.ContainsKey("Name") && !valuesAsDictionary.ContainsKey("name"))
            {
                valuesAsDictionary.Add("Name", Name); 
            }

            return new ObjectCreationSettingsCollection(mappings, configValues, onlySetPublicProperties);
        }

        /// <summary>
        /// If the user wants to access the ObjectCreationSettingsCollection object them selves then they should call this to get an instance of the settings.
        /// This will contain the information gained from the config and allow any further setup to be done before creating their final type object.
        /// will only set public properties.
        /// </summary>
        /// <returns></returns>
        public ObjectCreationSettingsCollection CreateCreationSettingsCollection()
        {
            return CreateCreationSettingsCollection(true);
        }

        /// <summary>
        /// If the user wants to access the ObjectCreationSettingsCollection object them selves then they should call this to get an instance of the settings.
        /// This will contain the information gained from the config and allow any further setup to be done before creating their final type object.
        /// </summary>
        /// <param name="onlySetPublicProperties"></param>
        /// <returns></returns>
        public ObjectCreationSettingsCollection CreateCreationSettingsCollection(bool onlySetPublicProperties)
        {
            return CreateCreationSettingsCollection(valuesAsDictionary, null, onlySetPublicProperties);
        }

        /// <summary>
        /// Allows a model to have its values overriden with the given config section, only values that exist in the config section will override properties in the model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public void Populate<T>(T model)
        {
            var settings = CreateCreationSettingsCollection(valuesAsDictionary, null, true);
            ObjectCreationAndPopulationFactory.PopulateFieldsFromValuesItems(model, settings.GetValidPropertySettings(), settings.OnlySetPublicProperties);
        }

        public static ConfigSection CreateSection<T>(T model)
        {
            throw new NotImplementedException();
        }
    }
}
