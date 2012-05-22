using System;

namespace ExampleApp.MoreComplexExamples.AutoGenerateModelsFromConfigWithDefaultValues
{
    /// <summary>
    /// A class to represent the business logic, or a controller from an MVC or MVVM project.
    /// </summary>
    public class DomainController
    {

        public DomainController()
        {
            DomainModel domainModelTemplateWithSettings = ConfigurationLoader.LoadDomainModelTemplateWithSettings();
            
            Console.WriteLine("Domain model has been auto generated, via a one line call in business domain,");
            Console.WriteLine("a default was given to the number of units of 123, if nothing is found in the config this is what would be used, (in this case it was removed from config to demonstrate this)");
            Console.WriteLine(string.Format("domain model name: '{0}' type: '{1}', number of units: {2} secrect number: {3}", 
                domainModelTemplateWithSettings.Name, 
                domainModelTemplateWithSettings.ModelType, 
                domainModelTemplateWithSettings.NumberUnits,
                domainModelTemplateWithSettings.GetMySecretNumberValue
            ));

            DomainModel domainModelTemplate = ConfigurationLoader.LoadDomainModelTemplate();
            Console.WriteLine();
            Console.WriteLine("loaded the same model but didn't use a settings class.  demonstrates that the properties can also be defaulted, within the constructor");
            Console.WriteLine(string.Format("domain model name: '{0}' secrect number '{1}'", domainModelTemplate.Name, domainModelTemplate.GetMySecretNumberValue));
            
            Console.WriteLine();
        }
    }
}
