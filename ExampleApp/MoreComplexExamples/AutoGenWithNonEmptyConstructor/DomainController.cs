using System;

namespace ExampleApp.MoreComplexExamples.AutoGenWithNonEmptyConstructor
{
    /// <summary>
    /// A class to represent the business logic, or a controller from an MVC or MVVM project.
    /// </summary>
    public class DomainController
    {

        public DomainController()
        {
            DomainModel domainModel = ConfigurationLoader.LoadDomainModelTemplate();
            
            Console.WriteLine("Domain model has been auto generated, via a one line call in business domain,");
            Console.WriteLine("values have been mapped and passed into the constructor as object does not have an empty constructor to use.");
            Console.WriteLine(string.Format("domain model name: '{0}' type: '{1}', number of units: {2} secrect number: {3}", 
                domainModel.Name, 
                domainModel.ModelType, 
                domainModel.NumberUnits,
                domainModel.GetMySecretNumberValue
            ));
            
            Console.WriteLine();
        }
    }
}
