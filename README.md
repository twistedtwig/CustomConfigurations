Custom Configurations:

Custom configurations is designed to reduce the repeative lines of code needed to get configuration settings into your application.  If there are 5 or 50 settings there doesn't need any extra code to get your settings.  It allows the application to simplify the use of configuration settings.  

A simple but effective use is to have an object with N number of properties, such as:

```C#
public class MyAppSettings
{
    public string Name { get; set; }
    public string Location { get; set; }
    public bool ShouldDoX { get; set; }
    public int minValue { get; set; }
}
```

Then use the configuration loader to populate the values:

```C#
MyAppSettings appSettings = new CustomConfigurations.Config().Create<MyAppSettings>();
```

Lets give a simple strongly typed example, lets say we have the following configuration file:

```xml
<?xml version="1.0"?>
<configuration>

  <configSections>
    <section name="domainModelTemplate" type="CustomConfigurations.ConfigurationSectionLoader, CustomConfigurations"/>
  </configSections>

  <domainModelTemplate>
    <Configs>
      <ConfigurationGroup name="model">
        <ValueItems>
          <ValueItem key="CanExecute" value="true"/>
          <ValueItem key="Description" value="domain model template desciption field"/>
          <ValueItem key="NumberUnits" value="5"/>
          <ValueItem key="ModelType" value="TheirType"/>
        </ValueItems>
      </ConfigurationGroup>
    </Configs>
  </domainModelTemplate>

<startup><supportedRuntime version="v2.0.50727"/></startup></configuration>
```

We want to load this model into memory:

```C#
    public class DomainModel
    {
        public string Name { get; set; }

        public bool CanExecute { get; set; }
        public string Description { get; set; }
        public int NumberUnits { get; set; }
        public DomainModelType ModelType { get; set; }        
    }

    public enum DomainModelType
    {
        MyType,
        TheirType,
        AnotherType,
    }
```

We can create a configuration load to isolate the use of custom configuration and simply return a strongly typed object to your business logic:

```C#
    public static class ConfigurationLoader
    {

        public static DomainModel LoadDomainModelTemplate()
        {
            return new Config("domainModelTemplate")
                .GetSection("model")
                .Create<DomainModel>();
        }
    }
```


Multipe sections example
------------------------

Another example is when the app has multiple clients settings to be recorded a section for each can be defined and the values seperated out. The config class allows easy access to a clients configuration and deals with the configuration hooking up in the background for you.

At this point it is not possible to save back to the configuration file. I have tried to get this to work across the board but failed misserably, will try to tackle this again at a later stage, (there are a few commented out sections and tests aimed at this in the committed code).

Here is an example configuration file:

```xml
<configuration>
  <configSections>
    <section name="testsection2" type="CustomConfigurations.ConfigurationSectionLoader, CustomConfigurations" />
    <sectionGroup name="myCustomGroup">
      <section name="mysection" type="CustomConfigurations.ConfigurationSectionLoader, CustomConfigurations" />
    </sectionGroup>
  </configSections>

  <myCustomGroup>
    <mysection>
      <Configs>
      <ConfigurationGroup name="client1">
        <ValueItems>
          <ValueItem key="key2" value="value2" />
          <ValueItem key="key3" value="value3" />
          <ValueItem key="key4" value="value4" />
          <ValueItem key="key5" value="7" />
          <ValueItem key="key6" value="0.6" />        
      </ConfigurationGroup>
      <ConfigurationGroup name="client2">
          <ValueItems>
            <ValueItem key="keya" value="abc" />
            <ValueItem key="key2" value="123" />
          </ValueItems>
        </ConfigurationGroup>
      </Configs>
    </mysection>
  </myCustomGroup>

  <testsection2>
    <Configs>
      <ConfigurationGroup name="clienta">
        <ValueItems>
          <ValueItem key="key2" value="valueabc" />          
        </ValueItems>       
      </ConfigurationGroup>
    </Configs>
  </testsection2>
</configuration>
```

```C#
CustomConfigurations.Config Configloader = new CustomConfigurations.Config("client2");
```

About the examples
------------------

The console application "ExampleApp" shows the different ways the customConfiguration application can be used. The examples include:

* Simply loading the configuration settings and manually getting a setting from it, (SimpleLoadingOfConfiguration.cs)
* Showing how the application will try and determine the custom section in the config file so you do not have to explicity declare it everytime (LoadingOfConfigurationByAllowingAppToDetermineConfigPath.cs).
* How to load multiple configuration sections at one time (LoadingMultipleConfigSections.cs)
* How to create more complex data structures in config, using inner collections (UsingInnerCollections.cs)
* How to load a different configuration file (LoadingAConfigFileThatIsNotTheDefaultConfig.cs)
* How to generate strongly typed objects automatically (ExampleApp.MoreComplexExamples.AutoGenerateModelsFromConfig)
* How to map configuration settings keys to the strongly typed object that have different property names (ExampleApp.MoreComplexExamples.MappingFieldsAutoGen)


Encrypting and decrypting
-------------------------

If required the whole configuration section can be encrypted.  The enryption can be used by multiple machines, (no machine key files used).  The encryption uses Triple-DES with a key file.  The first time it is to be used the key file needs to be generated.

```C#
Config config = new Config();
config.CreateConfigKey();
```

There are two ways to encrypt the configuration.

```C#
Config config = new Config();
config.EncryptConfigurationSection();
```

This will encrypt all custom config sections in the configuration file.

```C#
Config config = new Config();
config.EncryptConfigurationSection("sectionName");
```

To be able to do the encyption and decryption there is one addition to the configuration file:

```xml
  <configProtectedData>
    <providers>
      <add name="TripleDESProtectedConfigurationProvider" type="CustomConfigurations.TripleDESProtectedConfigurationProvider, CustomConfigurations" keyFilePath="configkey.txt" />
    </providers>
  </configProtectedData>
```

This will only encrypt the configuration section with the name "sectionName".

Thanks

-------------------------------------------------------------

N.B. all config sections are case sensitive, be careful how you write the xml and using the selectors.
