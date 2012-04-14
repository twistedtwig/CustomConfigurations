using NUnit.Framework;

namespace CustomConfigurations.Test
{
    [TestFixture]
    public class Collections
    {
        private CustomConfigurations.ConfigurationSectionLoader ConfigurationLoader;
        private ConfigurationGroupElement ConfigGroup;

        [SetUp]
        public void Init()
        {
            ConfigurationLoader = (CustomConfigurations.ConfigurationSectionLoader)System.Configuration.ConfigurationManager.GetSection("myCustomGroup/mysection");
            Assert.IsNotNull(ConfigurationLoader);
            ConfigGroup = ConfigurationLoader.ConfigGroups["client1"];
            Assert.IsNotNull(ConfigGroup);
        }


        [Test]
        public void TestCollectionsIsNotManditoryAndSectionsWillLoadWithoutThem()
        {
            CustomConfigurations.ConfigurationSectionLoader configurationLoader = (CustomConfigurations.ConfigurationSectionLoader) System.Configuration.ConfigurationManager.GetSection("myCustomGroup/mysection");
            Assert.IsNotNull(configurationLoader);

            ConfigurationGroupElement configGroup = configurationLoader.ConfigGroups["client1"];
            Assert.IsNotNull(configGroup);

            CollectionsGroupCollection collections = configGroup.InnerCollections;
            Assert.IsNotNull(collections);

            ConfigurationGroupElement collectionsGroup1 = collections["col1"];
            Assert.IsNotNull(collectionsGroup1);

            ConfigurationGroupElement collectionsGroup2 = collections["col2"];
            Assert.IsNotNull(collectionsGroup2);


            configurationLoader = (CustomConfigurations.ConfigurationSectionLoader) System.Configuration.ConfigurationManager.GetSection("testsection2");
            Assert.IsNotNull(configurationLoader);

            configGroup = configurationLoader.ConfigGroups["clienta"];
            Assert.IsNotNull(configGroup);

            collections = configGroup.InnerCollections;
            Assert.IsNotNull(collections);
            Assert.AreEqual(0, collections.Count);
        }

        [Test]
        public void TestThatCanGetAGivenInnerCollectionFromAKey()
        {
            ConfigurationGroupElement collection = ConfigGroup.InnerCollections["col2"];
            Assert.IsNotNull(collection);
            Assert.AreEqual("col2", collection.Name);
            Assert.AreEqual(3, collection.ValueItemCollection.Count);
        }

        [Test]
        public void TestThatCanGetValueFromInnerCollection()
        {
            ConfigurationGroupElement collection = ConfigGroup.InnerCollections["col1"];
            Assert.IsNotNull(collection);
            Assert.AreEqual("value3", collection.ValueItemCollection["key3"].Value);
        }

        [Test]
        public void TestThatGivenInInvalidInnerCollectionKeyWillReturnNull()
        {
            ConfigurationGroupElement collection = ConfigGroup.InnerCollections["col1"];
            Assert.IsNotNull(collection);
            Assert.IsNull(collection.ValueItemCollection["keyXYZ"]);
        }

        [Test]
        public void TestThatGienInValidCollectionGroupWillReturnNull()
        {
            ConfigurationGroupElement collection = ConfigGroup.InnerCollections["colXYZ"];
            Assert.IsNull(collection);
        }

        [Test]
        public void TestNestedInnerCollections()
        {
            ConfigurationGroupElement col2 = ConfigGroup.InnerCollections["col2"];
            Assert.IsNotNull(col2);

            ConfigurationGroupElement col3 = col2.InnerCollections["col3"];
            Assert.IsNotNull(col3);

            Assert.AreEqual("value2a", col3.ValueItemCollection["key2a"].Value);
            Assert.AreEqual("value3a", col3.ValueItemCollection["key3a"].Value);
        }
    }
}
