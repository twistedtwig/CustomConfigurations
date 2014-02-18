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

To access a value it would use the index of the key:
```C#
string myVal = Configloader["keya"];
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

Thanks

-------------------------------------------------------------

N.B. all config sections are case sensitive, be careful how you write the xml and using the selectors.
