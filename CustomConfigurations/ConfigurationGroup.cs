using System.Configuration;

namespace CustomConfigurations
{
    /// <summary>
    /// holds the collection of all configuraiton groups, i.e. all clients, the name attribute for each group is the key to the collection
    /// </summary>
    public class ConfigurationGroupCollection : ConfigurationElementCollection
    {
        public void Clear()
        {
            BaseClear();
        }

        public void Add(ConfigurationGroupElement item)
        {
            BaseAdd(item);
        }
        
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
            set { base[key] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public void Remove(ConfigurationGroupElement g)
        {
            BaseRemove(g.Name);
        }

        public void Remove(int index)
        {
            BaseRemoveAt(index);
        }
    }



    

    /// <summary>
    /// configuration group, one section per each client as such. contains the value items collection.
    /// </summary>
    public class ConfigurationGroupElement : ConfigurationElement
    {
        public int Index { get; set; }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        [ConfigurationProperty("ValueItems")]
        public ValueItemElementCollection ValueItemCollection
        {
            get { return this["ValueItems"] as ValueItemElementCollection; }
            set { this["ValueItems"] = value; }
        }

        [ConfigurationProperty("Collections", IsRequired = false)]
        public CollectionsGroupCollection InnerCollections
        {
            get { return this["Collections"] as CollectionsGroupCollection; }
            set { this["Collections"] = value; }
        }

    }
}
