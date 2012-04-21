using System;
using CustomConfigurations;

namespace ExampleApp
{
    public class CreatingStronglyTypedObjectsFromConfig
    {
        public CreatingStronglyTypedObjectsFromConfig()
        {
            ConfigSection configSection = new Config("myCustomGroup/mysection").GetSection("client1");

            MyObject myObj = new MyObject();

            myObj.Name = configSection.Name;
            myObj.Prop1 = configSection["key2"];
            myObj.Prop2 = configSection["key3"];
            myObj.Prop3 = configSection["key4"];

            Console.WriteLine("created a strongly typed object with string values of from config:");
            Console.WriteLine(myObj);

            Console.WriteLine("");
        }
    }

    public class MyObject
    {
        public string Name { get; set; }
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
        public string Prop3 { get; set; }

        public override string ToString()
        {
            return string.Format("My name is '{0}', prop1: {1}, prop2: {2}, prop3: {3}", Name, Prop1, Prop2, Prop3);
        }
    }
}
