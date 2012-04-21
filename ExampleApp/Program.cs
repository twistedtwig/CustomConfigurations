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
            CreatingStronglyTypedObjectsFromConfig cstofc = new CreatingStronglyTypedObjectsFromConfig();

            //wrapping configuration
            MoreComplexExamples.WrappingConfigurationIntoALoader.DomainController wdc = new MoreComplexExamples.WrappingConfigurationIntoALoader.DomainController();

            Console.WriteLine("all examples run, press any key to exit");
            Console.ReadKey();
        }
    }
}
