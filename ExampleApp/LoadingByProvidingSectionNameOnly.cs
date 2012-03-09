using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomConfigurations;

namespace ExampleApp
{
    public class LoadingByProvidingSectionNameOnly
    {
        /// <summary>
        /// Loading the config object can be done by providing only a section name rather than providing the whole config path of sectiongroup and sectionname.
        /// </summary>
        public LoadingByProvidingSectionNameOnly()
        {
            ConfigSection configSection = new Config("testsection2").GetSection("clienta");
            string myVal = configSection["key2"];

            Console.WriteLine("configloader loaded client section called 'clienta'");
            Console.WriteLine("value found for 'key' is: " + myVal);

            Console.WriteLine("");
        }
    }
}
