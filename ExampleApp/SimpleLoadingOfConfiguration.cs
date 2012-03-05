using System;
using CustomConfigurations;

namespace ExampleApp
{
    public class SimpleLoadingOfConfiguration
    {
        public SimpleLoadingOfConfiguration()
        {
            Config configLoader = new Config("client2");
            string myVal = configLoader["keya"];
            bool result;
            int myInt = configLoader.TryParse<int>("key2", out result);

            Console.WriteLine("configloader loaded client section called 'client2'");
            Console.WriteLine("value found for 'key' is: " + myVal);
            Console.WriteLine(string.Format("attempted try parse for 'key2' to convert to int result '{0}', value {1}", result, myInt));

            Console.WriteLine("");
        }
    }
}
