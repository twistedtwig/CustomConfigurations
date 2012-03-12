using System.Configuration;

namespace CustomConfigurations
{
    /// <summary>
    /// holds the collection of all configuraiton groups, i.e. all clients, the name attribute for each group is the key to the collection
    /// </summary>
    public class ConfigurationGroupCollection : ConfigurationElementCollection
    {
        
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurationGroupElement();
        }
        
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigurationGroupElement)element).Name;
        }

        protected override string ElementName
        {
            get { return "ConfigurationGroup"; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        public ConfigurationGroupElement this[int index]
        {
            get { return BaseGet(index) as ConfigurationGroupElement; }
        }

        public new ConfigurationGroupElement this[string key]
        {
            get { return BaseGet(key) as ConfigurationGroupElement; }
        }
    }



    

    /// <summary>
    /// configuration group, one section per each client as such. contains the value items collection.
    /// </summary>
    public class ConfigurationGroupElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return this["name"].ToString();
            }
        }
        
        [ConfigurationProperty("ValueItems")]        
        public ValueItemElementCollection ValueItemCollection
        {
            get { return this["ValueItems"] as ValueItemElementCollection; }
        }

        [ConfigurationProperty("Collections", IsRequired = false)]
        public CollectionsGroupCollection InnerCollections
        {
            get { return this["Collections"] as CollectionsGroupCollection; }
        }

    }
}
