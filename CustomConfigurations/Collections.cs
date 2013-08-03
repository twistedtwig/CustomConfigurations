using System.Configuration;

namespace CustomConfigurations
{
    /// <summary>
    /// Deals with Loading Configuration values from the config file.
    /// </summary>
    public class CollectionsSection : ConfigurationSection
    {
        /// <summary>
        /// Colleciton of ValueItemElements.
        /// </summary>
        [ConfigurationProperty("Collections")]
        public CollectionsGroupCollection ConfigGroups
        {
            get { return this["Collections"] as CollectionsGroupCollection; }
            set { this["Collections"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }


    /// <summary>
    /// Collection of the <c>CollectionsGroupElement</c> objects.
    /// </summary>
    public class CollectionsGroupCollection : ConfigurationElementCollection
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
            get { return "Collection"; }
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
    }

}
