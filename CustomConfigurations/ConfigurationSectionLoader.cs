﻿using System;
using System.Configuration;

namespace CustomConfigurations
{
    /// <summary>
    /// Deals with Loading Configuration values from the config file.
    /// </summary>
    public class ConfigurationSectionLoader : ConfigurationSection, IDisposable
    {
        /// <summary>
        /// Colleciton of ValueItemElements.
        /// </summary>
        [ConfigurationProperty("Configs")]        
        public ConfigurationGroupCollection ConfigGroups
        {
            get { return this["Configs"] as ConfigurationGroupCollection; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            
        }

        public void Save()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = config.GetSection("testsection2") as ConfigurationSectionLoader;

            section.ConfigGroups.Clear();

            foreach (ConfigurationGroupElement configGroup in this.ConfigGroups)
            {
                section.ConfigGroups.Add(configGroup);
            }



            config.Save(ConfigurationSaveMode.Full);
        }
    }

}
