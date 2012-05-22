
namespace CustomConfigurations.Test.DomainModels
{
    public class DefaultsDomainModel
    {
        public DefaultsDomainModel()
        {
            CanExecute = true;
            Description = "My Default Description";
            MySecretNumber = 3;
        }

        public string Name { get; set; }

        public bool CanExecute { get; set; }
        public string Description { get; set; }
        public int NumberUnits { get; set; }
        public DomainModelType ModelType { get; set; }
        public int MySecretNumber { get; set; }
    }
}

