using System.Collections.Generic;
using CustomConfigurations;

namespace ExampleApp.MoreComplexExamples.MappingFieldsAutoGen
{
    public static class ConfigurationLoader
    {

        public static AutoGenerateModelsFromConfig.DomainModel LoadDomainModelTemplate()
        {
            ConfigValueDictionary mappings = new ConfigValueDictionary();
            mappings.Add("NoUnits", "NumberUnits");

            return new Config(@"MoreComplexExamples\MappingFieldsAutoGen\mapping.config", "domainModelTemplate")
                .GetSection("model")
                .Create<AutoGenerateModelsFromConfig.DomainModel>(mappings, true);
        }
    }
}
