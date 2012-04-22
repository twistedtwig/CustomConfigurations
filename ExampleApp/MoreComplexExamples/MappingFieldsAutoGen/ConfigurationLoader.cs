using System.Collections.Generic;
using CustomConfigurations;

namespace ExampleApp.MoreComplexExamples.MappingFieldsAutoGen
{
    public static class ConfigurationLoader
    {

        public static AutoGenerateModelsFromConfig.DomainModel LoadDomainModelTemplate()
        {
            IDictionary<string, string> mappings = new Dictionary<string, string>();
            mappings.Add("NoUnits", "NumberUnits");

            return new Config(@"MoreComplexExamples\MappingFieldsAutoGen\mapping.config", "domainModelTemplate")
                .GetSection("model")
                .Create<AutoGenerateModelsFromConfig.DomainModel>(false, mappings);
        }
    }
}
