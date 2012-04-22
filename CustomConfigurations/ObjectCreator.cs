using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace CustomConfigurations
{
    /// <summary>
    /// Wrapper class to do some reflection creation and property initialization.
    /// </summary>
    public class ObjectCreator
    {
        /// <summary>
        /// Create an object of the given type T.  
        /// Can state if should only try and set public properties.
        /// Must pass in a dictionary of keys, the name of the property, and values the value as a string to be set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includePrivateorProtectedProperties"></param>
        /// <param name="valueDictionary"></param>
        /// <returns></returns>
        public static T Create<T>(bool includePrivateorProtectedProperties, IDictionary<string, string> valueDictionary)
        {
            return Create<T>(includePrivateorProtectedProperties, valueDictionary, null);
        }

        /// <summary>
        /// Create an object of the given type T.  
        /// Can state if should only try and set public properties.
        /// Must pass in a dictionary of keys, the name of the property, and values the value as a string to be set.
        /// if mappings are given, then the key it will look for will be the value in the mapping.. i.e. if mapping key = Prop1, mapping value = MyProp1 then when populating the fields from config
        /// it will not use the config key (Prop1) it will use the key mapping key (MyProp1) to search the object for that Property by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includePrivateorProtectedProperties"></param>
        /// <param name="valueDictionary"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public static T Create<T>(bool includePrivateorProtectedProperties, IDictionary<string, string> valueDictionary, IDictionary<string, string> mappings)
        {
            var obj = InstantiateObject<T>(valueDictionary);

            //populate values.
            PopulateFieldsFromValuesItems(obj, !includePrivateorProtectedProperties, valueDictionary, mappings);

            //return T
            return obj;
        }

        
        private static void PopulateFieldsFromValuesItems<T>(T obj, bool onlySetPublicSetters, IDictionary<string, string> valueDictionary, IDictionary<string, string>  mappings)
        {
            //foreach field in dictionary see if there is a field with the same name
            foreach (KeyValuePair<string, string> valuePair in valueDictionary)
            {
                string key = valuePair.Key;
                string value = valuePair.Value;

                if (mappings != null)
                {
                    if (mappings.ContainsKey(key))
                    {
                        key = mappings[key];
                    }
                }

                SetPropertyValue(obj, onlySetPublicSetters, key, value);
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

        private static T InstantiateObject<T>(IDictionary<string, string> valueDictionary)
        {
            ConstructorInfo constructor = DetermineBestConstructor<T>(valueDictionary);
            if (constructor == null) return default(T);

            //go through each paramater in constructor, get value from config by name and convert to parameter type and add into an object array.
            // create object
            ParameterInfo[] parameters = constructor.GetParameters();
            object[] paramArray = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                paramArray[i] = ConvertToType(valueDictionary[parameters[i].Name], parameters[i].ParameterType);
            }

            return (T)constructor.Invoke(paramArray);
        }

        private static ConstructorInfo DetermineBestConstructor<T>(IDictionary<string, string> values)
        {
            ConstructorInfo[] constructors = typeof(T).GetConstructors();
            if (constructors.Length == 0)
            {
                return null;
            }

            //ideally want to use the empty constructor
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length == 0) return constructor;
            }

            foreach (ConstructorInfo constructor in constructors)
            {
                bool allMatched = true;
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    if (values.Keys.Contains(parameter.Name))
                    {
                        if (!Is(values[parameter.Name], parameter.ParameterType)) allMatched = false;
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
