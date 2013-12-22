using System;
using System.Configuration;
using System.IO;
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
            var currentDirectory = Directory.GetCurrentDirectory();
            var exePath = Path.Combine(currentDirectory, "CustomConfigurations.Test.DLL");
            var path = Path.Combine(currentDirectory, "CustomConfigurations.Test.DLL.Config");
            var tempPath = path + "-temp";
            
            if (!File.Exists(tempPath))
            {
                File.Copy(path, tempPath);
            }
            else
            {
                File.Delete(path);
                File.Move(tempPath, path);
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
            ConfigurationLoader = (CustomConfigurations.ConfigurationSectionLoader)config.GetSection("myCustomGroup/mysection");
            Assert.IsNotNull(ConfigurationLoader);
        }

        [Test]
        public void TestThatConfigLoaderFindsValueCollectionAndLoadsItCorrectly()
        {
            Assert.AreEqual(2, ConfigurationLoader.ConfigGroups.Count);
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
            Assert.AreEqual(2, ConfigurationLoader.ConfigGroups.Count);
            ConfigurationGroupElement configGroup = ConfigurationLoader.ConfigGroups["clientXYZ"];
            Assert.IsNull(configGroup);
        }
        
        [Test]
        public void TestCanAddConfigSection()
        {
            Assert.AreEqual(2, ConfigurationLoader.ConfigGroups.Count);

            var element = new ConfigurationGroupElement();
            element.Name = "mynewelement123";

            ConfigurationLoader.ConfigGroups.Add(element);

            Assert.AreEqual(3, ConfigurationLoader.ConfigGroups.Count);
            var group = ConfigurationLoader.ConfigGroups[2];
            Assert.IsNotNull(group);

            Assert.AreEqual(element.Name, group.Name);

        }

        [Test]
        public void TestCanRemoveConfigSectionByKey()
        {
            Assert.AreEqual(2, ConfigurationLoader.ConfigGroups.Count);
            var group = ConfigurationLoader.ConfigGroups["client1"];

            Assert.IsNotNull(group);
            ConfigurationLoader.ConfigGroups.Remove(group);
            Assert.AreEqual(1, ConfigurationLoader.ConfigGroups.Count);
        }

        [Test]
        public void TestCanRemoveConfigSectionByIndex()
        {
            Assert.AreEqual(2, ConfigurationLoader.ConfigGroups.Count);

            ConfigurationLoader.ConfigGroups.Remove(0);
            Assert.AreEqual(1, ConfigurationLoader.ConfigGroups.Count);
        }

        [Test]
        public void TestCanClearRemoveAllSections()
        {
            Assert.AreEqual(2, ConfigurationLoader.ConfigGroups.Count);
            ConfigurationLoader.ConfigGroups.Clear();
            Assert.AreEqual(0, ConfigurationLoader.ConfigGroups.Count);

        }
    }
}
