using System;
using CustomConfigurations;

namespace ExampleApp
{
    public class LoadingAConfigFileThatIsNotTheDefaultConfig
    {
        public LoadingAConfigFileThatIsNotTheDefaultConfig()
        {
            ConfigSection configSection = new Config("app.Testfile.config", "testsection5").GetSection("clientb");
            string myVal = configSection["key23"];            

            Console.WriteLine("configloader loaded client section called 'clientb'");
            Console.WriteLine("value found for 'key23' is: " + myVal);

            Console.WriteLine("");
        }
    }
}
