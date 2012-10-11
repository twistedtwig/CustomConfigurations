
namespace CustomConfigurations
{
    public class ConfigValueItem
    {
        public ConfigValueItem(string key, string value) : this(key, value, false) { }

        public ConfigValueItem(string key, string value, bool inherited)
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
