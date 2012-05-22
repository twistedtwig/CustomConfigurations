
namespace CustomConfigurations.Test.DomainModels
{
    public class DomainModel
    {
        public DomainModel()
        {
            MySecretNumber = int.MinValue;
        }

        public string Name { get; set; }

        public bool CanExecute { get; set; }
        public string Description { get; set; }
        public int NumberUnits { get; set; }
        public DomainModelType ModelType { get; set; }
        private int MySecretNumber { get; set; }

        public int GetResultFromMySecretNumberPrivateSetter()
        {
            return MySecretNumber;
        }
    }

    public enum DomainModelType
    {
        MyType,
        TheirType,
        AnotherType,
    }
}
