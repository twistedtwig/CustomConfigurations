Custom Configurations:

For more detailed information please visit: https://github.com/twistedtwig/CustomConfigurations

Custom configurations is designed to reduce the repeative lines of code needed to get configuration settings into your application.  If there are 5 or 50 settings there doesn't need any extra code to get your settings.  It allows the application to simplify the use of configuration settings.  

A simple but effective use is to have an object with N number of properties, such as:

public class MyAppSettings
{
    public string Name { get; set; }
    public string Location { get; set; }
    public bool ShouldDoX { get; set; }
    public int minValue { get; set; }
}

Then use the configuration loader to populate the values:


MyAppSettings appSettings = new CustomConfigurations.Config().Create<MyAppSettings>();


Lets give a simple strongly typed example, lets say we have the following configuration file:


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


We want to load this model into memory:


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


We can create a configuration load to isolate the use of custom configuration and simply return a strongly typed object to your business logic:


    public static class ConfigurationLoader
    {

        public static DomainModel LoadDomainModelTemplate()
        {
            return new Config("domainModelTemplate")
                .GetSection("model")
                .Create<DomainModel>();
        }
    }



-------------------------------------------------------------

N.B. all config sections are case sensitive, be careful how you write the xml and using the selectors.


--------------------------------------------------------------

For more detailed information please visit: https://github.com/twistedtwig/CustomConfigurations