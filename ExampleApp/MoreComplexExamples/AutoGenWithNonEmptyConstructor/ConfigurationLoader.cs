using System.Collections.Generic;
using CustomConfigurations;

namespace ExampleApp.MoreComplexExamples.AutoGenWithNonEmptyConstructor
{
    public static class ConfigurationLoader
    {
        public static DomainModel LoadDomainModelTemplate()
        {
            ConfigSection configSection = new Config(@"MoreComplexExamples\AutoGenWithNonEmptyConstructor\autogenctor.config", "domainModelTemplate").GetSection("model");

            ConfigValueDictionary mappings = new ConfigValueDictionary();
            mappings.Add("mySecretNum", "MySecretNumber");  //pass in the param name for the constructor and the key of the corresponding ValueItem from config
            mappings.Add("numberUnits", "NumberUnits");    //remember that all params are case sensitive

            //if mappings had not been used it would not have known what values to use for both of these values which it got from config
            return configSection.Create<DomainModel>(mappings, true);
        }
    }
}
