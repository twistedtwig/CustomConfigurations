using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CustomConfigurations.Test
{
    [TestFixture]
    public class CollectionsGroup
    {
        private CustomConfigurations.Config ConfigLoader;
        private CustomConfigurations.ConfigSection ConfigurationSection;

        [SetUp]
        public void Init()
        {
            ConfigLoader = new CustomConfigurations.Config("myCustomGroup/mysection");
            Assert.IsNotNull(ConfigLoader);
            ConfigurationSection = ConfigLoader.GetSection("client1");            
            Assert.IsNotNull(ConfigurationSection);
        }

        [Test]
        public void TestCollectionsGroupReturnsCorrectInnerCollectionNames()
        {
            IEnumerable<string> sectionNames = ConfigurationSection.Collections.SectionNames;
            Assert.IsNotNull(sectionNames);
            IList<string> names = new List<string>(sectionNames);
            Assert.AreEqual(2, names.Count);
            Assert.Contains("col1", names.ToArray());
            Assert.Contains("col2", names.ToArray());
        }

        [Test]
        public void TesthasCollections()
        {
            Assert.IsTrue(ConfigurationSection.ContainsSubCollections);

            ConfigLoader = new CustomConfigurations.Config("testsection2");
            Assert.IsNotNull(ConfigLoader);
            ConfigurationSection = ConfigLoader.GetSection("clienta");
            Assert.IsNotNull(ConfigurationSection);

            Assert.IsFalse(ConfigurationSection.ContainsSubCollections);
        }

        [Test]
        public void TestHasCollections()
        {
            Assert.IsTrue(ConfigurationSection.Collections.HasCollections);

            CustomConfigurations.ConfigSection coll2 = ConfigurationSection.Collections.GetCollection("col2");
            Assert.IsNotNull(coll2);
            Assert.IsTrue(coll2.Collections.HasCollections);

            CustomConfigurations.ConfigSection coll3 = coll2.Collections.GetCollection("col3");
            Assert.IsNotNull(coll3);
            Assert.IsNull(coll3.Collections);
        }

        [Test]
        public void TestContainsKey()
        {
            Assert.IsTrue(ConfigurationSection.Collections.ContainsKey("col2"));
            Assert.IsFalse(ConfigurationSection.Collections.ContainsKey("colXYZ"));
        }

        [Test]
        public void TestReturningCorrectInnerCollection()
        {
            CustomConfigurations.ConfigSection innerSection = ConfigurationSection.Collections.GetCollection("col2");
            Assert.IsNotNull(innerSection);
            Assert.AreEqual("col2", innerSection.Name);
            Assert.AreEqual(3, innerSection.Count);
            Assert.AreEqual("8", innerSection["key5"]);
            Assert.IsTrue(innerSection.ContainsSubCollections);
        }

        [Test]
        public void TestNestedCollections()
        {
            CustomConfigurations.ConfigSection firstInner = ConfigurationSection.Collections.GetCollection("col2");
            Assert.IsNotNull(firstInner);
            CustomConfigurations.CollectionsGroup nestedInner = firstInner.Collections;
            Assert.IsNotNull(nestedInner);

            Assert.IsTrue(nestedInner.HasCollections);

            CustomConfigurations.ConfigSection coll3InnerNested = nestedInner.GetCollection("col3");
            Assert.IsNotNull(coll3InnerNested);
            Assert.IsNull(coll3InnerNested.Collections);
        }
    }
}
