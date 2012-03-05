using System;
using NUnit.Framework;

namespace CustomConfigurations.Test
{
    [TestFixture]
    public class Config
    {
        private const int TotalNumberOfValuePairs = 5;
        private CustomConfigurations.Config Configloader = null;

        [SetUp]
        public void Init()
        {           
            Configloader = new CustomConfigurations.Config("client1");
        }

        [Test]
        public void TestCanLoadConfigurationWithNoConfigurationPathGiven()
        {
            Configloader = new CustomConfigurations.Config("client1");
            Assert.IsNotNull(Configloader);
            Assert.AreEqual(TotalNumberOfValuePairs, Configloader.Count);
        }

        [Test]
        public void TestCanLoadConfigurationWithOnlySectionGiven()
        {
            Configloader = new CustomConfigurations.Config("testsection2", string.Empty);
            Assert.IsNotNull(Configloader);
            Assert.AreEqual(1, Configloader.Count);
        }

        [Test]
        public void TestCanDetermineSectionWithoutSectionGroup()
        {
            Configloader = new CustomConfigurations.Config();
            Assert.IsNotNull(Configloader);
            Assert.IsTrue(Configloader.ContainsKey("key2"));
            Assert.AreEqual(1, Configloader.Count);
            Assert.AreEqual("valueabc", Configloader["key2"]);
        }

        [Test]
        public void TestGivenAnAppPathCanConfigLoadAppSettings()
        {
            Assert.IsNotNull(Configloader);            
            Assert.AreEqual(TotalNumberOfValuePairs, Configloader.Count);
        }

        [Test]
        public void TestThatGivenAValidKeyItemWillValueWillBeReturned()
        {
            Assert.AreEqual("value4", Configloader["key4"]);
        }

        [Test]
        public void TestThatGivenAnINValidKeyANullObjectWillbeReturned()
        {
            Assert.IsNull(Configloader["keyXYZ"]);
        }

        [Test]
        public void TestGenericCastingOfValueToTypes()
        {
            bool result = false;
            int myInt = Configloader.TryParse<int>("key5", out result);
            Assert.IsNotNull(myInt);
            Assert.AreEqual(7, myInt);
            Assert.IsTrue(result);

            result = false;
            float myfloat = Configloader.TryParse<float>("key6", out result);
            Assert.IsNotNull(myfloat);
            Assert.AreEqual(0.6f, myfloat);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestGenericCastingOfInValidKeyReturnsNull()
        {
            bool result = true;
            int myInt = Configloader.TryParse<int>("keyXYZ", out result);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestContainsKey()
        {
            Assert.IsTrue(Configloader.ContainsKey("key2"));
            Assert.IsFalse(Configloader.ContainsKey("keyXYZ"));
        }
    }
}
