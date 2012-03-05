using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace CustomConfigurations
{
    /// <summary>
    /// The Config class is a wrapper around the ConfigurationSectionLoader.  It deals with creating and loading in the variables.
    /// It allows easier access to any Value for a given key.
    /// 
    /// For a given key it will return NULL or the value if found.
    /// </summary>
    public class Config
    {
        private ConfigurationSectionLoader ConfigSectionLoader;
        private ConfigurationGroupElement ConfigSection;

        /// <summary>
        /// Default constructor will give a blank configuration path and section, it will try and determine a the config path and choose the first valid section it can find.
        /// </summary>
        public Config() : this(string.Empty, string.Empty) { }

        /// <summary>
        /// If no configuration Path is given it will try and infer it.  Can only do this and be accurate if this configuration class is referenced once and only once in the config file.
        /// Otherwise it will choose the first reference and use that to load the values.
        /// </summary>
        public Config(string section) : this(string.Empty, section) { }

        /// <summary>
        /// Constructor with the full confiuration path given.
        /// </summary>
        /// <exception cref="ArgumentException">error if section is null or empty string</exception>
        /// <exception cref="ApplicationException">error if fails to load given configuration section</exception>
        /// <param name="configurationPath"></param>
        /// <param name="section"> </param>
        public Config(string configurationPath, string section)
        {            
            if (!string.IsNullOrEmpty(configurationPath))
            {
                //created the config SectionLoader with the given configuration path.
                if (CreateConfigurationLoaderObject(configurationPath))
                {
                    if (CreateConfigSection(section)) return;
                    //if we have had a valid configuration path but the section didn't work try and get a default.
                    if (TryAndAssignDefaultConfigSection()) return;
                }
            }
            if (ConfigSectionLoader == null) //if we have not yet created a configuration loader try again now.
            {
                foreach (string path in DetermineConfigurationPath())
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        //created the config SectionLoader with the given configuration path.
                        if (CreateConfigurationLoaderObject(path))
                        {
                            if (CreateConfigSection(section)) return;
                            //if no section was given and we have a valid configuration loader then try and get a default section.
                            if(string.IsNullOrEmpty(section) && TryAndAssignDefaultConfigSection()) return;
                        }
                    }
                }    
            }            

            if (TryAndAssignDefaultConfigSection()) return;

            //if we got here we were not able to create a configuraiton SectionLoader successfully.
            throw new ApplicationException("ConfigurationSectionLoader failed to load with any valid path");            
        }

        /// <summary>
        /// Try and assign a config section, try and assign the first section available, should only be used as last ditch.
        /// </summary>
        /// <returns></returns>
        private bool TryAndAssignDefaultConfigSection()
        {
            //if we got to this point it is most likely that no section was provided so just use the first one found.
            if (ConfigSectionLoader != null && ConfigSectionLoader.ConfigGroups != null && ConfigSectionLoader.ConfigGroups.Count > 0)
            {
                ConfigSection = ConfigSectionLoader.ConfigGroups[0];
                return true;
            }
            return false;
        }

        private IEnumerable<string> DetermineConfigurationPath()
        {
            string configFileLocation = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            string fullApplicationName = typeof (ConfigurationSectionLoader).FullName;
            string fullApplicationAssembly = typeof(ConfigurationSectionLoader).Assembly.ToString().Substring(0, typeof(ConfigurationSectionLoader).Assembly.ToString().IndexOf(",", StringComparison.InvariantCultureIgnoreCase));
            string configLoaderRegexString = fullApplicationName + @",[\s]{0,1}" + fullApplicationAssembly;
            Regex configRegex = new Regex(configLoaderRegexString);

            XPathDocument doc = new XPathDocument(configFileLocation);
            XPathNavigator nav = doc.CreateNavigator();

            XPathNodeIterator sectionIterator = nav.Select(@"configuration/configSections");
            foreach (var p in DetermineValidSectionElements(configRegex,String.Empty, sectionIterator)) yield return p;

            XPathNodeIterator sectionGroupIterator = nav.Select(@"configuration/configSections/sectionGroup");
            foreach (var p in DetermineValidSectionGroups(configRegex, sectionGroupIterator)) yield return p;
        }

        /// <summary>
        /// tries to find any valid section groups which contain the correct section to match the regex given for the transform type.
        /// </summary>
        /// <param name="configRegex"></param>
        /// <param name="sectionGroupIterator"></param>
        /// <returns></returns>
        private IEnumerable<string> DetermineValidSectionGroups(Regex configRegex, XPathNodeIterator sectionGroupIterator)
        {
            if (sectionGroupIterator.Count == 0)
            {
                throw new ApplicationException(
                    "unable to find the configuration sectionGroup within the applicaiton config file.");
            }

            while (sectionGroupIterator.MoveNext())
            {
                string sectionGroupName = sectionGroupIterator.Current.GetAttribute("name", "");

                string sectionGroupType = sectionGroupIterator.Current.GetAttribute("type", "");
                if (!string.IsNullOrEmpty(sectionGroupType))
                {
                    //have found a section handler that will use the configurationLoader to parse a custom config.
                    if (configRegex.IsMatch(sectionGroupType))
                    {
                        yield return sectionGroupName;
                    }
                }
                else //if the section group doesn't have the correct type then carry on searching.
                {
                    foreach (var p in DetermineValidSectionElements(configRegex, sectionGroupName, sectionGroupIterator)) yield return p;
                }
            }
        }

        /// <summary>
        /// Looks at the given iterator and tries to find any valid section groups that match the regex given for the config transform type.
        /// </summary>
        /// <param name="configRegex"></param>
        /// <param name="parentSection"></param>
        /// <param name="parentGroupIterator"></param>
        /// <returns></returns>
        private IEnumerable<string> DetermineValidSectionElements(Regex configRegex, string parentSection, XPathNodeIterator parentGroupIterator)
        {
            if (parentGroupIterator == null)
            {
                throw new ArgumentException("parentGroupIterator");
            }

            if (string.IsNullOrEmpty(parentSection.Trim()))
            {
                //first time round, so the parentGroupIterator has not been started
                parentGroupIterator.MoveNext();
            }

            XPathNodeIterator sectionIterator = parentGroupIterator.Current.Select(@"section");

            while (sectionIterator.MoveNext())
            {
                string sectionName = sectionIterator.Current.GetAttribute("name", "");
                string sectionType = sectionIterator.Current.GetAttribute("type", "");
                if (!string.IsNullOrEmpty(sectionType))
                {
                    //have found a section handler that will use the configurationLoader to parse a custom config.
                    if (configRegex.IsMatch(sectionType))
                    {
                        if (string.IsNullOrEmpty(parentSection))
                        {
                            yield return sectionName;                            
                        }
                        else
                        {
                            yield return string.Format("{0}/{1}", parentSection, sectionName);                            
                        }
                    }
                }
            }
        }

        /// <summary>
        /// creates the configurationLoader for the given path.
        /// </summary>
        /// <param name="configPath">path to the configuration section in the config file.</param>
        /// <returns>If the configuration Loader was created successfully, doesn't gaurantee items have been found, only that it is not null.</returns>
        private bool CreateConfigurationLoaderObject(string configPath)
        {
            if (string.IsNullOrEmpty(configPath))
            {
                throw new ArgumentException(configPath);
            }

            try
            {
                ConfigSectionLoader = ConfigurationManager.GetSection(configPath) as ConfigurationSectionLoader;
            }
            catch (Exception)
            {
                return false;
            }
            return ConfigSectionLoader != null;
        }

        /// <summary>
        /// Try and create the config Section object from the given section and the configloader already created.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        private bool CreateConfigSection(string section)
        {
            if (string.IsNullOrEmpty(section.Trim())) return false;

            ConfigSection = ConfigSectionLoader.ConfigGroups[section];
            if (ConfigSection != null)
            {
                //loaded a valid client configuration so happy to return at this point.
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the number of values found for that config group
        /// </summary>
        public int Count { get { return ConfigSection.ValueItemCollection.Count; } }

        /// <summary>
        /// Checks to see if a given key / value pair is present for the given config group.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return ConfigSection.ValueItemCollection[key] != null;
        }

        public string this[string key]
        {
            get
            {
                if (!ContainsKey(key))
                {
                    return null;
                }

                ValueItemElement item = ConfigSection.ValueItemCollection[key];
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
                    return (T) val;
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
