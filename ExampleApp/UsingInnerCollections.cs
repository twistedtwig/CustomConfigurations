using System;
using CustomConfigurations;

namespace ExampleApp
{
    public class UsingInnerCollections
    {
        /// <summary>
        /// Example of using the Collections and Collection elements, these can be nested as deep as required, 
        /// (n.b if doing deep nesting might want to consider if the current approach is the best fit).
        /// </summary>
        public UsingInnerCollections()
        {
            ConfigSection configSection = new Config("nestedCollections").GetSection("client1");

            ConfigSection innerCollection = configSection.Collections.GetCollection("col2");
            string myVal = innerCollection["key4"];

            ConfigSection nestedCollection = innerCollection.Collections.GetCollection("col3");
            string myNestedVal = nestedCollection["key2a"];

            Console.WriteLine("configloader loaded client section called 'client1'");
            Console.WriteLine("There is an inner collection called 'col2' which has a value for key: 'key4' of: " + myVal);
            Console.WriteLine("There is also a nested collection inside the inner collection called 'col3' with a value for key: 'key2a' of: " + myNestedVal);
            Console.WriteLine("The nesting of collections does not have a physical or logical limit, but deep nesting might be a sign that a rethink could be needed.");

            Console.WriteLine("");
        }
    }
}
