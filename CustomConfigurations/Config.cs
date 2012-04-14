using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
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
    public class Config : IDisposable
    {
        private ConfigurationSectionLoader ConfigSectionLoader;
        private  IList<string> ConfigSectionNames = new List<string>();
        

        /// <summary>
        /// Default constructor will give a blank configuration path and section, it will try and determine a the config path and choose the first valid section it can find.
        /// </summary>
        public Config() : this(string.Empty) { }

        /// <summary>
        /// Constructor with the full confiuration path given.
        /// </summary>
        /// <exception cref="ArgumentException">error if section is null or empty string</exception>
        /// <exception cref="ApplicationException">error if fails to load given configuration section</exception>
        /// <param name="configurationPath"></param>
        public Config(string configurationPath) : this(string.Empty, configurationPath)
        { }

        public Config(string pathToConfigFile, string configurationPath)
        {
            string filePath = pathToConfigFile.Trim();
            if (!string.IsNullOrEmpty(configurationPath))
            {
                //created the config SectionLoader with the given configuration path.
                if (CreateConfigurationLoaderObject(filePath, configurationPath))
                {
                    return;
                }
            }
            if (ConfigSectionLoader == null) //if we have not yet created a configuration loader try again now.
            {
                foreach (string path in DetermineConfigurationPath())
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        //created the config SectionLoader with the given configuration path.
                        if (CreateConfigurationLoaderObject(filePath, path))
                        {
                            return;
                        }
                    }
                }
            }

            //if we got here we were not able to create a configuraiton SectionLoader successfully.
            throw new ApplicationException("ConfigurationSectionLoader failed to load with any valid path");            
        }

        /// <summary>
        /// Tries to find all valid configuration section / group paths that are valid for this loader.
        /// </summary>
        /// <returns></returns>
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
        /// <param name="pathToConfigFile"> </param>
        /// <param name="configPath">path to the configuration section in the config file.</param>
        /// <returns>If the configuration Loader was created successfully, doesn't gaurantee items have been found, only that it is not null.</returns>
        private bool CreateConfigurationLoaderObject(string pathToConfigFile, string configPath)
        {
            if (string.IsNullOrEmpty(configPath))
            {
                throw new ArgumentException(configPath);
            }

            try
            {
                if (string.IsNullOrEmpty(pathToConfigFile.Trim()) || !File.Exists(pathToConfigFile))
                {
                    ConfigSectionLoader = ConfigurationManager.GetSection(configPath) as ConfigurationSectionLoader;    
                }
                else
                {
                    ConfigurationFileMap fileMap = new ConfigurationFileMap(pathToConfigFile); //Path to your config file
                    Configuration configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
                    ConfigSectionLoader = configuration.GetSection(configPath) as ConfigurationSectionLoader;    
                }
                
                if (ConfigSectionLoader == null) return false;

                foreach (ConfigurationGroupElement configGroup in ConfigSectionLoader.ConfigGroups)
                {
                    ConfigSectionNames.Add(configGroup.Name);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return ConfigSectionLoader != null;
        }

        /// <summary>
        /// Returns the number of Config Sections Found.
        /// </summary>
        public int Count { get { return ConfigSectionNames.Count; } }

        /// <summary>
        /// Returns a readonly collection of the section names available.
        /// </summary>
        public IEnumerable<string> SectionNames
        {
            get { return new ReadOnlyCollection<string>(ConfigSectionNames); }
        }

        /// <summary>
        /// Determines if the loader has a config section by the given name.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public bool ContainsKey(string sectionName)
        {
            return ConfigSectionNames.Contains(sectionName);
        }

        /// <summary>
        /// Get the config section by the name attribute.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public ConfigSection GetSection(string sectionName)
        {
            return !ConfigSectionNames.Contains(sectionName) ? null : new ConfigSection(ConfigSectionLoader.ConfigGroups[sectionName]);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {            
            ConfigSectionLoader.Dispose();
            ConfigSectionNames = null;
        }
//
//        public void Save()
//        {
//            throw new NotImplementedException();
//        }
//
//        public void SaveTest()
//        {
//            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
//
//            ConfigSection configSection = this.GetSection("clienta");
//            configSection["key2"] = "newvalue";
//
//            //save to apply changes
//            config.Save(ConfigurationSaveMode.Full);
//            //ConfigurationManager.RefreshSection("appSettings");
//
//        }
    }
}
