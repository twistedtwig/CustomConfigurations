﻿using CustomConfigurations.ObjectCreation;
using CustomConfigurations.Test.DomainModels;
using NUnit.Framework;

namespace CustomConfigurations.Test
{
    [TestFixture]
    public class ConfigSection
    {
        private CustomConfigurations.Config ConfigLoader;
        private CustomConfigurations.ConfigSection Section;

        [SetUp]
        public void Init()
        {
            ConfigLoader = new CustomConfigurations.Config("testsection2");
            Section = ConfigLoader.GetSection("clienta");
            Assert.IsNotNull(Section);
        }

        [Test]
        public void TestTValidNameIsReturnedForSectionGroup()
        {
            Assert.AreEqual("clienta", Section.Name);            
        }

        [Test]
        public void TestThatGivenAValidKeyItemWillValueWillBeReturned()
        {
            
            Assert.AreEqual("valueabc", Section["key2"]);

        }

        [Test]
        public void TestThatGivenAnINValidKeyANullObjectWillbeReturned()
        {
            Assert.IsNull(Section["keyXYZ"]);
        }

        [Test]
        public void TestGenericCastingOfValueToTypes()
        {
            ConfigLoader = new CustomConfigurations.Config("myCustomGroup/mysection");
            Section = ConfigLoader.GetSection("client1");
            Assert.IsNotNull(Section);

            bool result = false;
            int myInt = Section.TryParse<int>("key5", out result);
            Assert.IsNotNull(myInt);
            Assert.AreEqual(7, myInt);
            Assert.IsTrue(result);

            result = false;
            float myfloat = Section.TryParse<float>("key6", out result);
            Assert.IsNotNull(myfloat);
            Assert.AreEqual(0.6f, myfloat);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestGenericCastingOfInValidKeyReturnsNull()
        {
            bool result = true;
            int myInt = Section.TryParse<int>("keyXYZ", out result);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestContainsKey()
        {
            Assert.IsTrue(Section.ContainsKey("key2"));
            Assert.IsFalse(Section.ContainsKey("keyXYZ"));
        }

        [Test]
        public void TestReturningValuesAsDictionaryWorks()
        {
            ConfigLoader = new CustomConfigurations.Config("myCustomGroup/mysection");
            Section = ConfigLoader.GetSection("client1");

            ConfigValueDictionary dictionary = Section.ValuesAsDictionary;
            Assert.IsNotNull(dictionary);
            Assert.AreEqual(5, dictionary.Count);

            Assert.AreEqual("value2", dictionary["key2"]);
            Assert.AreEqual("value4", dictionary["key4"]);         
            Assert.AreEqual("0.6", dictionary["key6"]);         
        }


        /* test the collections section */

        [Test]
        public void TestContainsInnerCollection()
        {
            Assert.IsFalse(Section.ContainsSubCollections);

            //load the one that has collections
            ConfigLoader = new CustomConfigurations.Config("myCustomGroup/mysection");
            Section = ConfigLoader.GetSection("client1");

            Assert.IsTrue(Section.ContainsSubCollections);
        }

        [Test]
        public void TestParentLinksWork()
        {
            ConfigLoader = new CustomConfigurations.Config("myCustomGroup/mysection");
            Section = ConfigLoader.GetSection("client1");
            Assert.IsNotNull(Section);

            Assert.IsNull(Section.Parent);

            CustomConfigurations.ConfigSection childSection = Section.Collections.GetCollection("col2");
            Assert.IsNotNull(childSection);
            Assert.AreEqual(Section, childSection.Parent);
        }

        [Test]
        public void TestIsChildLink()
        {
            ConfigLoader = new CustomConfigurations.Config("myCustomGroup/mysection");
            Section = ConfigLoader.GetSection("client1");
            Assert.IsNotNull(Section);
            Assert.IsFalse(Section.IsChild);

            CustomConfigurations.ConfigSection childSection = Section.Collections.GetCollection("col2");
            Assert.IsNotNull(childSection);
            Assert.IsTrue(childSection.IsChild);
        }

        [Test]
        public void TestCanCreateTypedObjectAndPopulatePublicValuesFromValueItems()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("TypedDataConfig").GetSection("model");
            Assert.IsNotNull(configSection);

            DomainModel model = configSection.Create<DomainModel>();

            Assert.AreEqual("model", model.Name);
            Assert.IsTrue(model.CanExecute);
            Assert.AreEqual("domain model template desciption field", model.Description);
            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
            Assert.AreEqual(int.MinValue, model.GetResultFromMySecretNumberPrivateSetter());
        }

        [Test]
        public void TestCanCreateTypedObjectAndPopulatePublicAndPrivateValuesFromValueItems()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("TypedDataConfig").GetSection("model");
            Assert.IsNotNull(configSection);

            DomainModel model = configSection.Create<DomainModel>(false);

            Assert.AreEqual("model", model.Name);
            Assert.IsTrue(model.CanExecute);
            Assert.AreEqual("domain model template desciption field", model.Description);
            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
            Assert.AreEqual(2, model.GetResultFromMySecretNumberPrivateSetter());
        }

        [Test]
        public void TestCanCreateTypedObjectAndPopulateWithFieldMappings()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("TypedDataConfig").GetSection("model");
            Assert.IsNotNull(configSection);

            ConfigValueDictionary mappings = new ConfigValueDictionary();
            mappings.Add("NoUnits", "NumberUnits");

            DomainModel model = configSection.Create<DomainModel>(mappings, true);

            Assert.AreEqual("model", model.Name);
            Assert.IsTrue(model.CanExecute);
            Assert.AreEqual("domain model template desciption field", model.Description);
            Assert.AreEqual(23, model.NumberUnits);
            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
            Assert.AreEqual(int.MinValue, model.GetResultFromMySecretNumberPrivateSetter());
        }

        [Test]
        public void TestCanCreateObjectThatDoesntHaveEmptyConstructor()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("TypedDataConfig").GetSection("model");
            Assert.IsNotNull(configSection);

            ConfigValueDictionary mappings = new ConfigValueDictionary();
            mappings.Add("NoUnits", "NumberUnits");
            mappings.Add("name", "Name");
            mappings.Add("canExecute", "CanExecute");

            ComplexDomainModel model = configSection.Create<ComplexDomainModel>(mappings, true);

            Assert.AreEqual("model", model.GetResultOfConstructorSetName());
            Assert.IsTrue(model.GetResultOfConstructorSetCanExecute());
            Assert.AreEqual("domain model template desciption field", model.Description);
            Assert.AreEqual(23, model.NumberUnits);
            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
            Assert.AreEqual(int.MinValue, model.GetResultFromMySecretNumberPrivateSetter());
        }

        [Test]
        public void TestCanCreateObjectThatHasDefaultValuesAssignedInConstructorAnyDontGetOverwrittenIfNoSettingFound()
        {
            DefaultsDomainModel model = new CustomConfigurations.Config("DefaultedTypedDataConfig").GetSection("model").Create<DefaultsDomainModel>();

            Assert.AreEqual("model", model.Name);
            Assert.IsTrue(model.CanExecute);
            Assert.AreEqual("My Default Description", model.Description);
            Assert.AreEqual(5, model.NumberUnits);
            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
            Assert.AreEqual(3, model.MySecretNumber);
        }

        [Test]
        public void TestCanCreateObjectWithDefaultValuesUsingObjectCreationSettingsCollection()
        {
            const string description = "This is my Test description";
            const string numberUnitsStr = "4456";
            const int numberUnitsInt = 4456;

            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("MinDefaultedTypedDataConfig").GetSection("model");
            ObjectCreationSettingsCollection settings = configSection.CreateCreationSettingsCollection();
            settings.SetValue("NumberUnits", numberUnitsStr);
            settings.SetValue("Description", description);

            DefaultsDomainModel model = configSection.Create<DefaultsDomainModel>(settings);

            Assert.AreEqual("model", model.Name);
            Assert.IsTrue(model.CanExecute);
            Assert.AreEqual(description, model.Description);
            Assert.AreEqual(numberUnitsInt, model.NumberUnits);
            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
            Assert.AreEqual(2, model.MySecretNumber);
        }

        [Test]
        public void TestCanInheritValuesFromParent()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("inheritanceSection").GetSection("clienta");
            Assert.IsNotNull(configSection);

            Assert.IsTrue(configSection.ContainsSubCollections);
            Assert.AreEqual(1, configSection.ValuesAsDictionary.Count);
            string globalKey = "mykey1";
            Assert.IsTrue(configSection.ValuesAsDictionary.ContainsKey(globalKey));

            string globalValue = configSection.ValuesAsDictionary[globalKey];
            Assert.IsFalse(string.IsNullOrEmpty(globalValue));

            foreach (CustomConfigurations.ConfigSection childSection in configSection.Collections.GetCollections())
            {
                Assert.IsTrue(childSection.ValuesAsDictionary.ContainsKey(globalKey));
                Assert.AreEqual(globalValue, childSection.ValuesAsDictionary[globalKey]);
            }
        }

        [Test]
        public void TestCanInheritValuesFromParentCanBeTurnedOff()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("inheritanceSection", false).GetSection("clienta");
            Assert.IsNotNull(configSection);

            Assert.IsTrue(configSection.ContainsSubCollections);
            Assert.AreEqual(1, configSection.ValuesAsDictionary.Count);
            string globalKey = "mykey1";
            Assert.IsTrue(configSection.ValuesAsDictionary.ContainsKey(globalKey));

            string globalValue = configSection.ValuesAsDictionary[globalKey];
            Assert.IsFalse(string.IsNullOrEmpty(globalValue));

            foreach (CustomConfigurations.ConfigSection childSection in configSection.Collections.GetCollections())
            {
                Assert.IsFalse(childSection.ValuesAsDictionary.ContainsKey(globalKey));
            }
        }

        [Test]
        public void TestCanInheritValuesFromParentWenUsingGenericCreate()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("inheritanceSection").GetSection("clienta");
            Assert.IsNotNull(configSection);

            Assert.IsTrue(configSection.ContainsSubCollections);
            Assert.AreEqual(1, configSection.ValuesAsDictionary.Count);
            string globalKey = "mykey1";
            Assert.IsTrue(configSection.ValuesAsDictionary.ContainsKey(globalKey));

            string globalValue = configSection.ValuesAsDictionary[globalKey];
            Assert.IsFalse(string.IsNullOrEmpty(globalValue));            

            foreach (CustomConfigurations.ConfigSection childSection in configSection.Collections.GetCollections())
            {
                InheritanceTester test = childSection.Create<InheritanceTester>();
                if (test.Name.Equals("child1"))
                {
                    Assert.AreEqual(test.mykey1, globalValue);
                    Assert.AreEqual(test.Key2, "123");
                    Assert.IsTrue(string.IsNullOrEmpty(test.Key3));
                }

                if (test.Name.Equals("child2"))
                {
                    Assert.AreEqual(test.mykey1, globalValue);
                    Assert.AreEqual(test.Key3, "456");
                    Assert.IsTrue(string.IsNullOrEmpty(test.Key2));
                }
            }
        }

        [Test]
        public void TestCanInheritValuesFromParentWillNotOverwriteLocalValue()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("inheritanceSectionLocal").GetSection("clienta");
            Assert.IsNotNull(configSection);

            Assert.IsTrue(configSection.ContainsSubCollections);
            Assert.AreEqual(2, configSection.ValuesAsDictionary.Count);
            string globalKey = "mykey1";
            Assert.IsTrue(configSection.ValuesAsDictionary.ContainsKey(globalKey));

            string globalValue = configSection.ValuesAsDictionary[globalKey];
            Assert.IsFalse(string.IsNullOrEmpty(globalValue));

            CustomConfigurations.ConfigSection childSection = configSection.Collections.GetCollection("child");
            Assert.IsNotNull(childSection);
            Assert.IsTrue(childSection.ValuesAsDictionary.ContainsKey(globalKey));
            Assert.AreNotEqual(globalValue, childSection.ValuesAsDictionary[globalKey]);
            Assert.AreEqual("123", childSection.ValuesAsDictionary[globalKey]);
        }

        [Test]
        public void TestInheritedFlagIsSetCorrectly()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("inheritanceSectionLocal").GetSection("clienta");
            Assert.IsNotNull(configSection);

            string globalKey1 = "mykey1";
            string globalKey2 = "globalkey";
            Assert.IsTrue(configSection.ValuesAsDictionary.ContainsKey(globalKey1));
            string globalValue = configSection.ValuesAsDictionary[globalKey1];
            Assert.IsFalse(string.IsNullOrEmpty(globalValue));

            CustomConfigurations.ConfigSection childSection = configSection.Collections.GetCollection("child");
            Assert.IsNotNull(childSection);
            Assert.IsTrue(childSection.ValuesAsDictionary.ContainsKey(globalKey1));
            Assert.AreNotEqual(globalValue, childSection.ValuesAsDictionary[globalKey1]);
            Assert.AreEqual("123", childSection.ValuesAsDictionary[globalKey1]);

            ConfigValueDictionary values = childSection.ValuesAsDictionary;
            Assert.AreEqual(3, values.Count);

            string childKey = "mykey2";
            Assert.IsTrue(childSection.ValuesAsDictionary.ContainsKey(childKey));
            Assert.IsFalse(values.IsInherited(globalKey1));
            Assert.IsFalse(values.IsInherited(childKey));
            Assert.IsTrue(values.IsInherited(globalKey2));
        }

        [Test]
        public void TestCanPopulateExistingObjectAndOverrideSomeValues()
        {
            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("TypedDataConfig").GetSection("model");
            Assert.IsNotNull(configSection);

            var model = configSection.Create<DomainModel>();

            Assert.AreEqual("model", model.Name);
            Assert.IsTrue(model.CanExecute);
            Assert.AreEqual("domain model template desciption field", model.Description);
            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
            Assert.AreEqual(int.MinValue, model.GetResultFromMySecretNumberPrivateSetter());

            CustomConfigurations.ConfigSection subConfigSection = new CustomConfigurations.Config("SubTypedDataConfig").GetSection("subModel");
            Assert.IsNotNull(subConfigSection);

            subConfigSection.Populate(model);

            Assert.AreEqual("subModel", model.Name);
            Assert.IsFalse(model.CanExecute);
            Assert.AreEqual("my description", model.Description);
            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
            Assert.AreEqual(int.MinValue, model.GetResultFromMySecretNumberPrivateSetter());
        }


        //TODO this is not implemented yet!
//        [Test]
//        public void TestCanTakeAnObjectAndCreateConfigSectionFromIt()
//        {
//            CustomConfigurations.ConfigSection configSection = new CustomConfigurations.Config("TypedDataConfig").GetSection("model");
//            Assert.IsNotNull(configSection);
//
//            DomainModel model = configSection.Create<DomainModel>(false);
//
//            Assert.AreEqual("model", model.Name);
//            Assert.IsTrue(model.CanExecute);
//            Assert.AreEqual("domain model template desciption field", model.Description);
//            Assert.AreEqual(DomainModelType.TheirType, model.ModelType);
//            Assert.AreEqual(2, model.GetResultFromMySecretNumberPrivateSetter());
//
//            var configSectionFromModel = CustomConfigurations.ConfigSection.CreateSection(model);
//
//            Assert.IsNotNull(configSectionFromModel);
//            Assert.AreEqual(configSection.Count, configSectionFromModel.Count);
//            Assert.AreEqual(configSection.IsChild, configSectionFromModel.IsChild);            
//        }
    }
}
