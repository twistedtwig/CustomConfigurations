using System;
using CustomConfigurations;

namespace ExampleApp
{
    public class SimpleLoadingOfConfiguration
    {
        /// <summary>
        /// Loading a specified Section by giving the path to the custom config and the section name.
        /// </summary>
        public SimpleLoadingOfConfiguration()
        {
            ConfigSection configSection = new Config("myCustomGroup/mysection").GetSection("client1");
            string myVal = configSection["key2"];
            bool result;
            int myInt = configSection.TryParse<int>("key5", out result);

            Console.WriteLine("configloader loaded client section called 'client2'");
            Console.WriteLine("value found for 'key2' is: " + myVal);
            Console.WriteLine(string.Format("attempted try parse for 'key5' to convert to int result '{0}', value {1}", result, myInt));

            Console.WriteLine("");
        }
    }
}
