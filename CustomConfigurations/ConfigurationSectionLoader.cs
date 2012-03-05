using System.Configuration;

namespace CustomConfigurations
{
    /// <summary>
    /// Deals with Loading Configuration values from the config file.
    /// </summary>
    public class ConfigurationSectionLoader : ConfigurationSection
    {
        /// <summary>
        /// Colleciton of ValueItemElements.
        /// </summary>
        [ConfigurationProperty("Configs")]        
        public ConfigurationGroupCollection ConfigGroups
        {
            get { return this["Configs"] as ConfigurationGroupCollection; }
        }

    }


    



}
