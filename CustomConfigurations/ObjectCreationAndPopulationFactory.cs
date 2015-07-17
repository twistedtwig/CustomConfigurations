using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CustomConfigurations.ObjectCreation
{
    /// <summary>
    /// Wrapper class to do some reflection creation and property initialization.
    /// </summary>
    public class ObjectCreationAndPopulationFactory
    {
        public static T Create<T>()
        {
            return Create<T>(new ObjectCreationSettingsCollection());
        }

        public static T Create<T>(ObjectCreationSettingsCollection creationSettings)
        {
            if (creationSettings == null)
                throw new ArgumentException("no creation settings provided.");


            var obj = InstantiateObject<T>(creationSettings.GetValidConstructorSettings());

            //populate values.
            PopulateFieldsFromValuesItems(obj, creationSettings.GetValidPropertySettings(), creationSettings.OnlySetPublicProperties);

            //return T
            return obj;
        }

        public static void PopulateFieldsFromValuesItems<T>(T obj, IList<ObjectCreationSettingItem> propertySettings, bool onlySetPublicProperties)
        {
            //foreach field see if there is a field with the same name
            foreach (ObjectCreationSettingItem creationSettingItem in propertySettings)
            {
                SetPropertyValue(obj, onlySetPublicProperties, creationSettingItem.MapToName, creationSettingItem.Value);   
            }            
        }

        private static void SetPropertyValue<T>(T obj, bool onlySetPublicSetters, string key, string value)
        {
            PropertyInfo prop = GetProperty(obj, key, onlySetPublicSetters);
            if (prop != null)
            {
                if (Is(value, prop.PropertyType))
                {
                    prop.SetValue(obj, ConvertToType(value, prop.PropertyType), null);
                }
            }
        }

        /// <summary>
        /// Returns the <c>PropertyInfo</c> object for the given object and the property name
        /// </summary>
        /// <param name="objectToGetProperty"></param>
        /// <param name="propertyName"></param>
        /// <param name="onlyPublic"> </param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(Object objectToGetProperty, string propertyName, bool onlyPublic)
        {
            if (objectToGetProperty == null) { throw new ArgumentException("objectToGetProperty was NULL"); }
            if (propertyName == null) { throw new ArgumentException("propertyName was NULL"); }

            PropertyInfo publicPropertyInfo = objectToGetProperty.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (publicPropertyInfo != null)
            {
                if (onlyPublic)
                {
                    if (publicPropertyInfo.GetSetMethod() != null)
                    {
                        return publicPropertyInfo;
                    }
                }
                return publicPropertyInfo;
            }

            return !onlyPublic ? objectToGetProperty.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic) : null;
        }

        private static T InstantiateObject<T>(IList<ObjectCreationSettingItem> constructorSettings)
        {
            ConstructorInfo constructor = DetermineBestConstructor<T>(constructorSettings);
            if (constructor == null) return default(T);

            //go through each paramater in constructor, get value from config by name and convert to parameter type and add into an object array.
            // create object
            ParameterInfo[] parameters = constructor.GetParameters();
            object[] paramArray = new object[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                //go through all constructor settingItems and create the object array with typed objects.
                paramArray[i] = ConvertToType(FindMatchingSetting(parameters[i].Name, constructorSettings).Value, parameters[i].ParameterType);
            }

            return (T)constructor.Invoke(paramArray);
        }

        private static ObjectCreationSettingItem FindMatchingSetting(string parameterName, IList<ObjectCreationSettingItem> constructorSettings)
        {
            ObjectCreationSettingItem orignalNameMatch = constructorSettings.FirstOrDefault(x => x.OriginalName.Equals(parameterName));
            if (orignalNameMatch != null) { return orignalNameMatch; }

            ObjectCreationSettingItem mapToNameMatch = constructorSettings.FirstOrDefault(x => x.MapToName.Equals(parameterName));
            if (mapToNameMatch != null) { return mapToNameMatch; }

            throw new ArgumentException("no constructor creation setting found for parameter: " + parameterName);
        }

        private static ConstructorInfo DetermineBestConstructor<T>(IList<ObjectCreationSettingItem> constructorSettings)
        {
            ConstructorInfo[] constructors = typeof(T).GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();
            if (constructors.Length == 0)
            {
                return null;
            }

            foreach (ConstructorInfo constructor in constructors)
            {
                bool allMatched = true;
                ParameterInfo[] parameters = constructor.GetParameters();
                foreach (ParameterInfo parameter in parameters)
                {
                    if (!constructorSettings.Any(settingItem => 
                        (settingItem.OriginalName.Equals(parameter.Name) || settingItem.MapToName.Equals(parameter.Name))
                            && Is(settingItem.Value, parameter.ParameterType)
                            && (settingItem.CreationSettingType == ObjectCreationSettingType.ConstructorOrProperty || settingItem.CreationSettingType == ObjectCreationSettingType.ConstructorOnly)))
                    {
                        allMatched = false;
                    }
                }

                if (allMatched) return constructor;
            }

            return null;
        }

        private static bool Is(string input, Type targetType)
        {
            try
            {
                TypeDescriptor.GetConverter(targetType).ConvertFromString(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static object ConvertToType(string input, Type t)
        {
            if (!Is(input, t)) return null;

            var converter = TypeDescriptor.GetConverter(t);
            return converter.ConvertFromString(input);
        }

    }
}
