using System.IO;
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

        [Test]
        public void TestCanLoadFileFromThatIsNotDefaultConfigPath()
        {
            //Copy test file so it can be reused many times:

            string tempFilePath = "app.Testfile.config";
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
            File.Copy("App.Test.config", tempFilePath);
            Assert.IsTrue(File.Exists(tempFilePath));

            Configloader = new CustomConfigurations.Config(tempFilePath, "testsection5");
            Assert.IsNotNull(Configloader);
            Assert.AreEqual(3, Configloader.Count);            
        }

        [Test]
        public void TestWhenGivenIncorrectFilePathWillDefaultBackToNormalConfigFile()
        {
            Configloader = new CustomConfigurations.Config("C:\temp\thisIsARubbishFilePath.config" ,"testsection2");
            Assert.IsNotNull(Configloader);
            Assert.AreEqual(2, Configloader.Count);            
        }
    }
}
