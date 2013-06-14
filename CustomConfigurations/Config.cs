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
        private readonly string ConfigurationPath;
        private ConfigurationSectionLoader ConfigSectionLoader;
        private  IList<string> ConfigSectionNames = new List<string>();
        private readonly bool AllowValueInheritance;

        /// <summary>
        /// Default constructor will give a blank configuration path and section, it will try and determine a the config path and choose the first valid section it can find.
        /// </summary>
        public Config() : this(string.Empty, true) { }
        public Config(bool allowValueInheritance) : this(string.Empty, allowValueInheritance) { }

        /// <summary>
        /// Constructor with the full configuration path given.
        /// </summary>
        /// <exception cref="ArgumentException">error if section is null or empty string</exception>
        /// <exception cref="ApplicationException">error if fails to load given configuration section</exception>
        /// <param name="pathToConfigFile"> </param>
        /// <param name="configurationPath">The xml path in the config file.</param>
        public Config(string pathToConfigFile, string configurationPath) : this(pathToConfigFile, configurationPath, true)
        {
        }

        /// <summary>
        /// Constructor with the full configuration path given.
        /// </summary>
        /// <exception cref="ArgumentException">error if section is null or empty string</exception>
        /// <exception cref="ApplicationException">error if fails to load given configuration section</exception>
        /// <param name="configurationPath">The xml path in the config file.</param>
        public Config(string configurationPath) : this(string.Empty, configurationPath, true)
        { }

        /// <summary>
        /// Constructor with the full configuration path given.
        /// </summary>
        /// <exception cref="ArgumentException">error if section is null or empty string</exception>
        /// <exception cref="ApplicationException">error if fails to load given configuration section</exception>
        /// <param name="configurationPath">The xml path in the config file.</param>
        /// <param name="allowValueInheritance"> </param>
        public Config(string configurationPath, bool allowValueInheritance) : this(string.Empty, configurationPath, allowValueInheritance)
        { }

        public Config(string pathToConfigFile, string configurationPath, bool allowValueInheritance)
        {
            AllowValueInheritance = allowValueInheritance;
            var filePath = pathToConfigFile.Trim();
            if (!string.IsNullOrEmpty(configurationPath))
            {
                //created the config SectionLoader with the given configuration path.
                if (CreateConfigurationLoaderObject(filePath, configurationPath, allowValueInheritance))
                {
                    return;
                }
            }
            if (ConfigSectionLoader == null) //if we have not yet created a configuration loader try again now.
            {
                foreach (string path in DetermineConfigurationPath(pathToConfigFile))
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        //created the config SectionLoader with the given configuration path.
                        if (CreateConfigurationLoaderObject(filePath, path, allowValueInheritance))
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
        private IEnumerable<string> DetermineConfigurationPath(string pathToConfigFile)
        {
            var configFileLocation = String.Empty;            
            if (!string.IsNullOrEmpty(pathToConfigFile))
            {
                configFileLocation = pathToConfigFile.Trim();
            }
            try
            {
                configFileLocation = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            }
            catch { /*  tried to open as stand alone app  */ }

            if (!File.Exists(configFileLocation))
            {
                configFileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.config");                
            }

            if (!File.Exists(configFileLocation))
            {
                configFileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web.config");                
            }
                
            if (!File.Exists(configFileLocation))
            {
                throw new FileNotFoundException(configFileLocation);
            }
            
            var fullApplicationName = typeof (ConfigurationSectionLoader).FullName;
            var fullApplicationAssembly = typeof(ConfigurationSectionLoader).Assembly.ToString().Substring(0, typeof(ConfigurationSectionLoader).Assembly.ToString().IndexOf(",", StringComparison.InvariantCultureIgnoreCase));
            var configLoaderRegexString = fullApplicationName + @",[\s]{0,1}" + fullApplicationAssembly;
            var configRegex = new Regex(configLoaderRegexString);

            var doc = new XPathDocument(configFileLocation);
            var nav = doc.CreateNavigator();

            var sectionIterator = nav.Select(@"configuration/configSections");
            if(sectionIterator.Count > 0)
            {
                foreach (var p in DetermineValidSectionElements(configRegex,String.Empty, sectionIterator)) yield return p;
            }

            var sectionGroupIterator = nav.Select(@"configuration/configSections/sectionGroup");
            if(sectionGroupIterator.Count > 0)
            {
                foreach (var p in DetermineValidSectionGroups(configRegex, sectionGroupIterator)) yield return p;
            }
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
                throw new ApplicationException("unable to find the configuration sectionGroup within the applicaiton config file.");
            }

            while (sectionGroupIterator.MoveNext())
            {
                var sectionGroupName = sectionGroupIterator.Current.GetAttribute("name", "");

                var sectionGroupType = sectionGroupIterator.Current.GetAttribute("type", "");
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

            var sectionIterator = parentGroupIterator.Current.Select(@"section");

            while (sectionIterator.MoveNext())
            {
                var sectionName = sectionIterator.Current.GetAttribute("name", "");
                var sectionType = sectionIterator.Current.GetAttribute("type", "");
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
        /// <param name="allowValueInheritance">should all child configsection's elements inherit from their parent?</param>
        /// <returns>If the configuration Loader was created successfully, doesn't gaurantee items have been found, only that it is not null.</returns>
        private bool CreateConfigurationLoaderObject(string pathToConfigFile, string configPath, bool allowValueInheritance)
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
                    var fileMap = new ConfigurationFileMap(pathToConfigFile); //Path to your config file
                    var configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
                    ConfigSectionLoader = configuration.GetSection(configPath) as ConfigurationSectionLoader;
                }

                if (ConfigSectionLoader == null) return false;

                foreach (ConfigurationGroupElement configGroup in ConfigSectionLoader.ConfigGroups)
                {
                    ConfigSectionNames.Add(configGroup.Name);
                }

                SetIndexesForConfigGroupCollectionRecursively(ConfigSectionLoader.ConfigGroups);                
            }
            catch (ConfigurationErrorsException)
            {
                //error with configuration inside what seems to be a valid section, rethrow.
                throw;
            }
            catch (Exception ex)
            {
                //no good, try more options.
                return false;
            }
            return ConfigSectionLoader != null;
        }

        
      
        private void SetIndexesForConfigGroupCollectionRecursively(ConfigurationGroupCollection configGroupCollection)
        {
            for (int i = 0; i < configGroupCollection.Count; i++)
            {
                var configGroup = configGroupCollection[i];
                configGroup.Index = i;

                SetIndexesForConfigGroupElementRecursively(configGroup);
            }            
        }

        private void SetIndexesForConfigGroupElementRecursively(ConfigurationGroupElement configGroup)
        {
            for (int j = 0; j < configGroup.ValueItemCollection.Count; j++)
            {
                configGroup.ValueItemCollection[j].Index = j;
            }

            if (configGroup.InnerCollections != null && configGroup.InnerCollections.Count > 0)
            {
                for (int i = 0; i < configGroup.InnerCollections.Count; i++)
                {
                    var innerConfigGroup = configGroup.InnerCollections[i];
                    innerConfigGroup.Index = i;

                    SetIndexesForConfigGroupElementRecursively(innerConfigGroup);
                }
            }
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
            return !ConfigSectionNames.Contains(sectionName) ? null : new ConfigSection(ConfigSectionLoader.ConfigGroups[sectionName], AllowValueInheritance);
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
