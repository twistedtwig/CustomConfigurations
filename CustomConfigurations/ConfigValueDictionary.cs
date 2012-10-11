using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomConfigurations
{
    public class ConfigValueDictionary : IEnumerable<ConfigValueItem>
    {
        private readonly IDictionary<string, ConfigValueItem> Mappings = new Dictionary<string, ConfigValueItem>();

        public bool ContainsKey(string key)
        {
            return Mappings.ContainsKey(key);
        }

        public void Add(string key, string value)
        {
            Add(key, value, false);
        }

        public void Add(string key, string value,  bool overRideExisting)
        {
            Add(key, value, overRideExisting, false);
        }
        
        public void Add(string key, string value,  bool overRideExisting, bool inherited)
        {
            if (Mappings.ContainsKey(key) && !overRideExisting) { return; }

            if (Mappings.ContainsKey(key) && overRideExisting)
            {
                Mappings[key] = new ConfigValueItem(key, value, inherited);
            }
            else
            {
                Mappings.Add(key, new ConfigValueItem(key, value, inherited));
            }
        }
        
        public bool Remove(string key)
        {
            return Mappings.Remove(key);
        }


        public string this[string key]
        {
            get { return Mappings[key].Value; }
        }
        
        public void Clear()
        {
            Mappings.Clear();            
        }

        public bool IsInherited(string key)
        {
            if(!ContainsKey(key)) throw new ArgumentOutOfRangeException("key");

            return Mappings[key].IsInherited;
        }

        public int Count
        {
            get { return Mappings.Count; }
        }

        public IEnumerator<ConfigValueItem> GetEnumerator()
        {
            return Mappings.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<string> Values
        {
            get { return Mappings.Values.Select(v => v.Value).AsEnumerable(); }
        }
    }
}
