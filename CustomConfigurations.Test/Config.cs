using System.Linq;
using NUnit.Framework;

namespace CustomConfigurations.Test
{
    [TestFixture]
    public class Config
    {
        private CustomConfigurations.Config Configloader;

        [SetUp]
        public void Init()
        {
            Configloader = new CustomConfigurations.Config("myCustomGroup/mysection");
        }

        [Test]
        public void TestCanLoadConfigurationWithNoConfigurationPathGiven()
        {
            Configloader = new CustomConfigurations.Config();
            Assert.IsNotNull(Configloader);
            Assert.AreEqual(2, Configloader.Count);
        }

        [Test]
        public void TestCanLoadConfigurationWithOnlySectionGiven()
        {
            Configloader = new CustomConfigurations.Config("testsection2");
            Assert.IsNotNull(Configloader);
            Assert.AreEqual(2, Configloader.Count);
        }
        
        [Test]
        public void TestGivenAnAppPathCanConfigLoadAppSettings()
        {
            Assert.IsNotNull(Configloader);            
            Assert.AreEqual(1, Configloader.Count);
        }       

        [Test]
        public void TestContainsKey()
        {
            Assert.IsTrue(Configloader.ContainsKey("client1"));
            Assert.IsFalse(Configloader.ContainsKey("clientXYZ"));
        }

        [Test]
        public void TestCanGetSection()
        {
            Assert.IsNotNull(Configloader.GetSection("client1"));
            Assert.IsNull(Configloader.GetSection("client1XYZ"));
        }

        [Test]
        public void TestCanGetAllSectionNames()
        {
            Configloader = new CustomConfigurations.Config("testsection2");
            Assert.IsNotNull(Configloader);           

            Assert.Contains("clienta", Configloader.SectionNames.ToArray());
            Assert.Contains("clientb", Configloader.SectionNames.ToArray());
        }
    }
}
