using NUnit.Framework;

namespace CustomConfigurations.Test
{
    [TestFixture]
    public class ConfigurationSectionLoader
    {
        private CustomConfigurations.ConfigurationSectionLoader ConfigurationLoader;

        [SetUp]
        public void Init()
        {
            ConfigurationLoader = (CustomConfigurations.ConfigurationSectionLoader)System.Configuration.ConfigurationManager.GetSection("myCustomGroup/mysection");
            Assert.IsNotNull(ConfigurationLoader);
        }

        [Test]
        public void TestThatConfigLoaderFindsValueCollectionAndLoadsItCorrectly()
        {
            Assert.AreEqual(1, ConfigurationLoader.ConfigGroups.Count);
            ConfigurationGroupElement configGroup = ConfigurationLoader.ConfigGroups["client1"];
            Assert.IsNotNull(configGroup);
            Assert.AreEqual("client1", configGroup.Name);
            Assert.AreEqual(5, configGroup.ValueItemCollection.Count);

            ValueItemElement item2 = configGroup.ValueItemCollection["key2"];
            Assert.IsNotNull(item2);
            Assert.AreEqual("value2", item2.Value);

            ValueItemElement item3 = configGroup.ValueItemCollection["key3"];
            Assert.IsNotNull(item3);
            Assert.AreEqual("value3", item3.Value);

            ValueItemElement item4 = configGroup.ValueItemCollection["key4"];
            Assert.IsNotNull(item4);
            Assert.AreEqual("value4", item4.Value);

            ValueItemElement item5 = configGroup.ValueItemCollection["key5"];
            Assert.IsNotNull(item5);
            Assert.AreEqual("7", item5.Value);

            ValueItemElement item6 = configGroup.ValueItemCollection["key6"];
            Assert.IsNotNull(item6);
            Assert.AreEqual("0.6", item6.Value);
        }

        [Test]
        public void TestWhenInvalidKeyUsedToFindConfigGroupNullObjectReturnedNoErrorThrown()
        {
            Assert.AreEqual(1, ConfigurationLoader.ConfigGroups.Count);
            ConfigurationGroupElement configGroup = ConfigurationLoader.ConfigGroups["clientXYZ"];
            Assert.IsNull(configGroup);
        }

    }
}
