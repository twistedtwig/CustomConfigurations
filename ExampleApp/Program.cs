using System;

namespace ExampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleLoadingOfConfiguration slc = new SimpleLoadingOfConfiguration();
            LoadingOfConfigurationByAllowingAppToDetermineConfigPath lcadc = new LoadingOfConfigurationByAllowingAppToDetermineConfigPath();
            LoadingByProvidingSectionNameOnly lpso = new LoadingByProvidingSectionNameOnly();
            LoadingMultipleConfigSections lmc = new LoadingMultipleConfigSections();
            UsingInnerCollections uic = new UsingInnerCollections();
            LoadingAConfigFileThatIsNotTheDefaultConfig lcf = new LoadingAConfigFileThatIsNotTheDefaultConfig();

            Console.WriteLine("all examples run, press any key to exit");
            Console.ReadKey();
        }
    }
}
