﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomConfigurations.ObjectCreation
{
    public class ObjectCreationSettingsCollection 
    {
        private IList<ObjectCreationSettingItem> SettingItems;

        /// <summary>
        /// No mappings given to the constructor, will only set public properties
        /// </summary>
        internal ObjectCreationSettingsCollection() : this(new ConfigValueDictionary(), true) { }
        /// <summary>
        /// Maps the dictionry key to the dictionary value for a field, only set public properties.
        /// </summary>
        /// <param name="fieldMappings"></param>
        internal ObjectCreationSettingsCollection(ConfigValueDictionary fieldMappings) : this(fieldMappings, true) { }

        internal ObjectCreationSettingsCollection(ConfigValueDictionary fieldMappings, bool onlySetPublicProperties) : this(fieldMappings, null, onlySetPublicProperties) { }

        internal ObjectCreationSettingsCollection(ConfigValueDictionary fieldMappings, ConfigValueDictionary fieldValues, bool onlySetPublicProperties)
        {
            OnlySetPublicProperties = onlySetPublicProperties;
            SettingItems = new List<ObjectCreationSettingItem>();

            //setup the field mappings
            OnlySetPublicProperties = onlySetPublicProperties;
            SettingItems = new List<ObjectCreationSettingItem>();

            if (fieldValues != null)
            {
                SetupValues(fieldValues);
            }
                      
            //add any overrides and values as required.
            if (fieldMappings != null)
            {
                SetupMappings(fieldMappings);
            }
        }

        public ObjectCreationSettingsCollection(IList<ObjectCreationSettingItem> mappings)
        {
            SettingItems = mappings ?? new List<ObjectCreationSettingItem>();
        }

        private void SetupMappings(ConfigValueDictionary fieldMappings)
        {
            foreach (ConfigValueItem item in fieldMappings)
            {
                if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                {
                    if (ContainsOriginalMappingName(item.Key))
                    {
                        SettingItems.First(x => x.OriginalName.Equals(item.Key)).MapToName = item.Value;  
                    }
                    if (ContainsMapToName(item.Value))
                    {
                        SettingItems.First(x => x.MapToName.Equals(item.Value)).OriginalName = item.Key;                                                                        
                    }
                    else
                    {
                        SettingItems.Add(
                            (
                                new ObjectCreationSettingItem()
                                {
                                    OriginalName = item.Key,
                                    MapToName = item.Value,
                                    CreationSettingType = ObjectCreationSettingType.ConstructorOrProperty
                                }
                            )
                        );
                    }
                }
            }
        }

        private void SetupValues(ConfigValueDictionary fieldValues)
        {
            foreach (ConfigValueItem item in fieldValues)
            {
                if (!string.IsNullOrEmpty(item.Key))
                {
                    if (!ContainsOriginalMappingName(item.Key))
                    {
                        SettingItems.Add(
                            (
                                new ObjectCreationSettingItem()
                                    {
                                        OriginalName = item.Key,
                                        MapToName = item.Key,
                                        DefaultValue = item.Value,
                                        OriginalValue = item.Value,
                                        CreationSettingType = ObjectCreationSettingType.ConstructorOrProperty
                                    }
                            )

                        );
                    }
                    else
                    {
                        SetValue(item.Key, item.Value);
                    }
                }
            }
        }

        public bool OnlySetPublicProperties { get; set; }


        public void AddMapping(string originalName, string mapToName)
        {
            AddMapping(originalName, mapToName, null);
        }

        public void AddMapping(string originalName, string mapToName, string defaultValue)
        {
            AddMapping(originalName, mapToName, defaultValue, null);
        }

        public void AddMapping(string originalName, string mapToName, string defaultValue, ObjectCreationSettingType? creationType)
        {
            if (string.IsNullOrEmpty(mapToName.Trim()))
                throw new ArgumentException("mapToName name null or empty");

            var setting = GetSettingItem(originalName);
                       
            if (setting != null)
            {
                setting.MapToName = mapToName;
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    setting.DefaultValue = defaultValue;
                }

                if (creationType.HasValue)
                {
                    setting.CreationSettingType = creationType.Value;
                }                
            }
            else
            {
                SettingItems.Add(new ObjectCreationSettingItem()
                                     {
                                         OriginalName = originalName,
                                         MapToName = mapToName,
                                         DefaultValue = !string.IsNullOrEmpty(defaultValue) ? defaultValue : String.Empty,
                                         CreationSettingType = creationType.HasValue ? creationType.Value : ObjectCreationSettingType.ConstructorOrProperty
                                     });
            }
        }

        private ObjectCreationSettingItem GetSettingItem(string originalName)
        {
            if (string.IsNullOrEmpty(originalName.Trim()))
                throw new ArgumentException("original name null or empty");

            if (SettingItems.Any(x => x.OriginalName.Equals(originalName)))
            {
                return SettingItems.FirstOrDefault(x => x.OriginalName.Equals(originalName));
            }

            return null;
        }

        /// <summary>
        /// Will override the original value and default value for any item already in the collection with that original name
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="value"></param>
        public void SetValue(string originalName, string value)
        {
            var setting = GetSettingItem(originalName);
            if (setting != null)
            {
                setting.OriginalValue = value;
                setting.DefaultValue = value;
            }
            else
            {
                AddMapping(originalName, originalName, value);
            }
        }

        public bool ContainsOriginalMappingName(string name)
        {
            return SettingItems.Any(x => x.OriginalName.Equals(name));
        }

        public bool ContainsMapToName(string name)
        {
            return SettingItems.Any(x => x.MapToName.Equals(name));
        }

        public IList<ObjectCreationSettingItem> GetValidConstructorSettings()
        {
            return GetValidConstructorSettings(false);
        }

        public IList<ObjectCreationSettingItem> GetValidConstructorSettings(bool allowNullorEmptyValues)
        {
            return SettingItems.Where(item => item.CreationSettingType == ObjectCreationSettingType.ConstructorOrProperty || item.CreationSettingType == ObjectCreationSettingType.ConstructorOnly)
                .Where(item => allowNullorEmptyValues || !string.IsNullOrEmpty(item.Value)).ToList();
        }

        public IList<ObjectCreationSettingItem> GetValidPropertySettings()
        {
            return GetValidPropertySettings(false);
        }

        public IList<ObjectCreationSettingItem> GetValidPropertySettings(bool allowNullorEmptyValues)
        {
            return SettingItems.Where(item => item.CreationSettingType == ObjectCreationSettingType.ConstructorOrProperty || item.CreationSettingType == ObjectCreationSettingType.PropertyOnly)
                .Where(item => allowNullorEmptyValues || !string.IsNullOrEmpty(item.Value)).ToList();
        }
    }
}
