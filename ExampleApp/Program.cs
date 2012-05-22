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

            //auto generate object from config file.
            MoreComplexExamples.AutoGenerateModelsFromConfig.DomainController autoGen = new MoreComplexExamples.AutoGenerateModelsFromConfig.DomainController();

            //auto generate object from config file, with mapping values.
            MoreComplexExamples.MappingFieldsAutoGen.DomainController mappingGen = new MoreComplexExamples.MappingFieldsAutoGen.DomainController();

            MoreComplexExamples.AutoGenerateModelsFromConfigWithDefaultValues.DomainController autoGenWithDefaults = new MoreComplexExamples.AutoGenerateModelsFromConfigWithDefaultValues.DomainController();


            Console.WriteLine("all examples run, press any key to exit");
            Console.ReadKey();
        }
    }
}
