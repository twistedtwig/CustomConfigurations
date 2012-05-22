
namespace CustomConfigurations.Test.DomainModels
{
    public class ComplexDomainModel
    {
        public ComplexDomainModel(string name, bool canExecute)
        {
            MySecretNumber = int.MinValue;
            Name = name;
            CanExecute = canExecute;
        }

        private string Name;
        private bool CanExecute;

        public string Description { get; set; }
        public int NumberUnits { get; set; }
        public DomainModelType ModelType { get; set; }
        private int MySecretNumber { get; set; }

        public int GetResultFromMySecretNumberPrivateSetter()
        {
            return MySecretNumber;
        }

        public string GetResultOfConstructorSetName()
        {
            return Name;
        }

        public bool GetResultOfConstructorSetCanExecute()
        {
            return CanExecute;
        }
    }

}
