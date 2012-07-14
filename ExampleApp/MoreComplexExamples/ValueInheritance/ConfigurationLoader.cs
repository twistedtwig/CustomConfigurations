using System.Collections.Generic;
using CustomConfigurations;

namespace ExampleApp.MoreComplexExamples.ValueInheritance
{
    public static class ConfigurationLoader
    {
        public static IList<Task> LoadTasks()
        {
            ConfigSection section = new Config(@"MoreComplexExamples\ValueInheritance\ValueInheritance.config", "domainModelTemplate").GetSection("model");

            IList<Task> Tasks = new List<Task>();
            foreach (ConfigSection childSection in section.Collections.GetCollections())
            {
                Tasks.Add(childSection.Create<Task>());    
            }

            return Tasks;
        }
    }
}
