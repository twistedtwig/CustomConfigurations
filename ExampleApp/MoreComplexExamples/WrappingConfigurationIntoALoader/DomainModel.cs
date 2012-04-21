using System.Collections.Generic;

namespace ExampleApp.MoreComplexExamples.WrappingConfigurationIntoALoader
{
    public class DomainModel
    {
        public DomainModel()
        {
            Contacts = new List<DomainContact>();
        }
        public string Name { get; set; }

        public bool CanExecute { get; set; }
        public string Description { get; set; }
        public int NumberUnits { get; set; }
        public DomainModelType ModelType { get; set; }
        public IList<DomainContact> Contacts { get; set; }
    }

    public class DomainContact
    {
        public string Name { get; set; }
    }

    public enum DomainModelType
    {
        MyType,
        TheirType,
        AnotherType,
    }
}
