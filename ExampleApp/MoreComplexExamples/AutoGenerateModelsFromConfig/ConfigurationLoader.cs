using CustomConfigurations;

namespace ExampleApp.MoreComplexExamples.AutoGenerateModelsFromConfig
{
    public static class ConfigurationLoader
    {

        public static DomainModel LoadDomainModelTemplate()
        {
            return new Config(@"MoreComplexExamples\AutoGenerateModelsFromConfig\autogen.config", "domainModelTemplate")
                .GetSection("model")
                .Create<DomainModel>();
        }
    }
}
