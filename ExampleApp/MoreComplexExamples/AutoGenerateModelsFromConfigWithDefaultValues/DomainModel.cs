
namespace ExampleApp.MoreComplexExamples.AutoGenerateModelsFromConfigWithDefaultValues
{
    public class DomainModel
    {
        public DomainModel()
        {
            MySecretNumber = 444;
        }

        public string Name { get; set; }

        public bool CanExecute { get; set; }
        public string Description { get; set; }
        public int NumberUnits { get; set; }
        public DomainModelType ModelType { get; set; }
        private int MySecretNumber { get; set; }

        public int GetMySecretNumberValue { get { return MySecretNumber; } }
    }

    public enum DomainModelType
    {
        MyType,
        TheirType,
        AnotherType,
    }
}
