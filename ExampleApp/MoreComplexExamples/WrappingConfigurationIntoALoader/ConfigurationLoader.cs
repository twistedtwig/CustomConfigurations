using System;
using CustomConfigurations;

namespace ExampleApp.MoreComplexExamples.WrappingConfigurationIntoALoader
{
    public static class ConfigurationLoader
    {

        public static DomainModel LoadDomainModelTemplate()
        {
            ConfigSection configSection = new Config(@"MoreComplexExamples\WrappingConfigurationIntoALoader\wrapping.config", "domainModelTemplate").GetSection("model");

            DomainModel model = new DomainModel();
            model.Name = configSection.Name;

            bool canExecute;
            model.CanExecute = bool.TryParse(configSection["ShouldExecute"], out canExecute) && canExecute;

            if (!string.IsNullOrEmpty(configSection["description"]))
            {
                model.Description = configSection["description"];
            }

            int noUnits;
            model.NumberUnits = Int32.TryParse(configSection["noUnits"], out noUnits) ? noUnits : 0;


            try
            {
                DomainModelType modelType = (DomainModelType)Enum.Parse(typeof(DomainModelType), configSection["domainType"]);
                model.ModelType = modelType;
            }
            catch
            {
                model.ModelType = DomainModelType.MyType;
            }

            if (configSection.ContainsSubCollections)
            {
                if (configSection.Collections.ContainsKey("Contacts"))
                {
                    ConfigSection contacts = configSection.Collections.GetCollection("Contacts");
                    foreach (string contactName in contacts.ValuesAsDictionary.Values)
                    {
                        model.Contacts.Add(new DomainContact { Name = contactName });
                    }
                }
            }

            return model;
        }
    }
}
