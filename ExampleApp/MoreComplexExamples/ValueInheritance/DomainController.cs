using System;
using System.Collections.Generic;

namespace ExampleApp.MoreComplexExamples.ValueInheritance
{
    /// <summary>
    /// A class to represent the business logic, or a controller from an MVC or MVVM project.
    /// </summary>
    public class DomainController
    {

        public DomainController()
        {
            Console.WriteLine("Tasks have been loaded,");
            
            IList<Task> Tasks = ConfigurationLoader.LoadTasks();
            foreach (Task task in Tasks)
            {
                Console.WriteLine("Task has name: '{0}', CanExecute {1}, and Value {2}", task.Name, task.CanExecute, task.Value);
            }
            Console.WriteLine();            
            Console.WriteLine();
        }
    }
}
