using System;
using CustomConfigurations;

namespace ExampleApp
{
    public class LoadingOfConfigurationByAllowingAppToDetermineConfigPath
    {
        /// <summary>
        /// The loading of the config object did not specify a config path so the application determined the first valid path it could find.
        /// below is the configSections segment of the app config file.  It chooses to use "testsection2" becuase it appears first in the list.
        /// If this had been declared after the sectiongroup element or not at all then it would have choosen the section group "myCustomGroup".
        /// 
        /// configSections
        ///     section name="testsection2" type="CustomConfigurations.ConfigurationSectionLoader, CustomConfigurations"
        ///     sectionGroup name="myCustomGroup">
        ///         section name="mysection" type="CustomConfigurations.ConfigurationSectionLoader, CustomConfigurations"
        ///         sectionGroup
        /// configSections
        /// 
        /// </summary>
        public LoadingOfConfigurationByAllowingAppToDetermineConfigPath()
        {
            ConfigSection configSection = new Config().GetSection("clienta");
            string myVal = configSection["key2"];            

            Console.WriteLine("configloader loaded client section called 'clienta'");
            Console.WriteLine("value found for 'key' is: " + myVal);

            Console.WriteLine("");
        }
    }
}
