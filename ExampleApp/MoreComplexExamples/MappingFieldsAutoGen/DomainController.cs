using System;

namespace ExampleApp.MoreComplexExamples.MappingFieldsAutoGen
{
    /// <summary>
    /// A class to represent the business logic, or a controller from an MVC or MVVM project.
    /// </summary>
    public class DomainController
    {

        public DomainController()
        {
            AutoGenerateModelsFromConfig.DomainModel domainModelTemplate = ConfigurationLoader.LoadDomainModelTemplate();
            
            Console.WriteLine("Domain model has been auto generated with mapped values, number of units didn't match config name,");
            Console.WriteLine("only took one call in config loader to get information.");
            Console.WriteLine(string.Format("domain model name: '{0}' type: '{1}', number of units: {2}", 
                domainModelTemplate.Name, 
                domainModelTemplate.ModelType, 
                domainModelTemplate.NumberUnits)
            );

            Console.WriteLine();
        }
    }
}
