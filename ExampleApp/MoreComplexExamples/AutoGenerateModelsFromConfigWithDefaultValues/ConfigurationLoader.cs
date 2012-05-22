using CustomConfigurations;
using CustomConfigurations.ObjectCreation;

namespace ExampleApp.MoreComplexExamples.AutoGenerateModelsFromConfigWithDefaultValues
{
    public static class ConfigurationLoader
    {
        public static DomainModel LoadDomainModelTemplate()
        {
            return new Config(@"MoreComplexExamples\AutoGenerateModelsFromConfigWithDefaultValues\autogendefaults.config", "domainModelTemplate")
                .GetSection("model")
                .Create<DomainModel>();
        }

        public static DomainModel LoadDomainModelTemplateWithSettings()
        {
            ConfigSection configSection = new Config(@"MoreComplexExamples\AutoGenerateModelsFromConfigWithDefaultValues\autogendefaults.config", "domainModelTemplate").GetSection("model");
            ObjectCreationSettingsCollection creationSettings = configSection.CreateCreationSettingsCollection(false); //set private properties as well.
            creationSettings.SetValue("NumberUnits", "123");

            return configSection.Create<DomainModel>(creationSettings);
        }
    }
}
