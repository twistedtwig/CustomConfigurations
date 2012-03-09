using System;
using CustomConfigurations;

namespace ExampleApp
{
    public class LoadingMultipleConfigSections
    {
        /// <summary>
        /// Loads the config object and gets two different configSections to be able to get variables key values across both sections.
        /// </summary>
        public LoadingMultipleConfigSections()
        {
            Config config = new Config("myCustomGroup/mysection");

            foreach (var sectionName in config.SectionNames)
            {
                Console.WriteLine("section name key found: " + sectionName);
            }

            ConfigSection configSection1 = config.GetSection("client1");
            ConfigSection configSection2 = config.GetSection("client2");

            string myVal1 = configSection1["key2"];
            string myVal2 = configSection2["key2"];

            Console.WriteLine("loaded two client objects, client1 and client2");
            Console.WriteLine("found values for both, each using the same key, ('key2')");
            Console.WriteLine("client 1 key val: " + myVal1);
            Console.WriteLine("client 2 key val: " + myVal2);
            Console.WriteLine("");
        }
    }
}
