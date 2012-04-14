using System.IO;
using NUnit.Framework;

namespace CustomConfigurations.Test
{
    [TestFixture]
    public class ValueItems
    {
        private const string TempFilePath = "app.Testfile.config";
        private const string SourceAppConfig = "App.Test.config";
        private CustomConfigurations.Config Configloader;
        private CustomConfigurations.ConfigSection ClientSection;

        [SetUp]
        public void Init()
        {
            //Copy test file so it can be reused many times:
            if (File.Exists(TempFilePath))
            {
                File.Delete(TempFilePath);
            }
            Assert.IsFalse(File.Exists(TempFilePath));
            Assert.IsTrue(File.Exists(SourceAppConfig));
            File.Copy(SourceAppConfig, TempFilePath);
            Assert.IsTrue(File.Exists(TempFilePath));

            Configloader = new CustomConfigurations.Config(TempFilePath, "testsection5");
            Assert.IsNotNull(Configloader);
            ClientSection = Configloader.GetSection("clienta");
            Assert.IsNotNull(ClientSection);
        }

        [TearDown]
        public void Dispose()
        {
            //clean up after ones self! :p
            File.Delete(TempFilePath);
        }

//        private void SaveConfigAndReloadVariables()
//        {
//            //save config
//            Configloader.Save();
//
//            //null out configloader
//            ClientSection = null;
//            Configloader.Dispose();
//            Configloader = null;
//
//            //load it new again
//            Configloader = new CustomConfigurations.Config(TempFilePath, "testsection5");
//            ClientSection = Configloader.GetSection("clienta");
//        }
//
//
//        [Test]
//        public void TestThatCanUpdateAValueInConfigFile()
//        {
//            const string newValueToUse ="mynewvalue";
//            const string key = "key2";
//
//            //get a value                      
//            string val = ClientSection[key];
//            Assert.AreEqual("valueabc", val);
//
//            //update it
//            ClientSection[key] = newValueToUse;
//
//            SaveConfigAndReloadVariables();
//
//            //assert that value is the new value.
//            string newVal = ClientSection[key];
//            Assert.AreEqual(newValueToUse, newVal);
//        }
//
//
//        [Test]
//        public void TestThatCanAddNewValueToConfigCollection()
//        {
//            const string newValue = "bleh";
//            const string key = "MyNewKey";
//
//            Assert.IsFalse(ClientSection.ContainsKey(key));
//
//            ClientSection[key] = newValue;
//            Assert.IsTrue(ClientSection.ContainsKey(key));
//
//            //save the file, close it and reopen it to ensure it is still there.
//            SaveConfigAndReloadVariables();
//            Assert.IsTrue(ClientSection.ContainsKey(key));
//
//        }
//
//        [Test]
//        public void TestThatCanRemoveValueItemFromConfigCollection()
//        {
//            const string key = "key2";
//
//            //get a value                    
//            Assert.IsTrue(ClientSection.ContainsKey(key));
//            string val = ClientSection[key];
//            Assert.AreEqual("valueabc", val);
//
//            ClientSection.Remove(key);
//            Assert.IsFalse(ClientSection.ContainsKey(key));
//        }
    }
}
