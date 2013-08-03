﻿using System;
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
            Assert.AreEqual(2, Configloader.Count);
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
            //clean up after ones self! :p
            File.Delete(tempFilePath);
        }

        [Test]
        public void TestWhenGivenIncorrectFilePathWillDefaultBackToNormalConfigFile()
        {
            Configloader = new CustomConfigurations.Config("C:\temp\thisIsARubbishFilePath.config" ,"testsection2");
            Assert.IsNotNull(Configloader);
            Assert.AreEqual(2, Configloader.Count);            
        }

        [Test]
        public void TestConfigurationLoaderAssignsAllIndexesCorrectly()
        {            
            CustomConfigurations.ConfigSection configSection = Configloader.GetSection("client1");
            Assert.AreEqual(0, configSection.Index);

            Assert.AreEqual(0, configSection.GetValueItemIndex("key2"));
            Assert.AreEqual(2, configSection.GetValueItemIndex("key4"));
            Assert.AreEqual(1, configSection.GetValueItemIndex("key3"));
            Assert.AreEqual(3, configSection.GetValueItemIndex("key5"));
            Assert.AreEqual(4, configSection.GetValueItemIndex("key6"));

            CustomConfigurations.ConfigSection innerCol1 = configSection.Collections.GetCollection("col1");
            Assert.AreEqual(0, innerCol1.Index);
            Assert.AreEqual(0, innerCol1.GetValueItemIndex("key2"));
            Assert.AreEqual(1, innerCol1.GetValueItemIndex("key3"));

            CustomConfigurations.ConfigSection innerCol2 = configSection.Collections.GetCollection("col2");
            Assert.AreEqual(1, innerCol2.Index);
            Assert.AreEqual(1, innerCol2.GetValueItemIndex("key4"));
            Assert.AreEqual(0, innerCol2.GetValueItemIndex("key3"));
            Assert.AreEqual(2, innerCol2.GetValueItemIndex("key5"));

            CustomConfigurations.ConfigSection innerCol3 = innerCol2.Collections.GetCollection("col3");
            Assert.AreEqual(0, innerCol3.Index);
            Assert.AreEqual(0, innerCol3.GetValueItemIndex("key2a"));
            Assert.AreEqual(1, innerCol3.GetValueItemIndex("key3a"));

            //Test other config section as it has multiple config groups section
            Configloader = new CustomConfigurations.Config("testsection2");
            Assert.IsNotNull(Configloader);

            Assert.AreEqual(0, Configloader.GetSection("clienta").Index);
            Assert.AreEqual(1, Configloader.GetSection("clientb").Index);
        }

//        [Test]
//        public void TestCanDisguardChangesMadeThatHaveNotBeenSaved()
//        {
//            throw new NotImplementedException();
//        }
//
//        [Test]
//        public void TestSave()
//        {
//            Configloader = new CustomConfigurations.Config("App.Test.Config","testsection5");
//            Configloader.SaveTest();
//        }
    }
}
