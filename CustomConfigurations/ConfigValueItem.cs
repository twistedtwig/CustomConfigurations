
namespace CustomConfigurations
{
    public class ConfigValueItem
    {
        public ConfigValueItem(string key, string value, bool inherited = false)
        {
            Key = key;
            Value = value;
            IsInherited = inherited;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsInherited { get; set; }
    }
}
