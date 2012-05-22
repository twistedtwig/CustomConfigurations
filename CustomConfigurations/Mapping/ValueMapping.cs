
namespace CustomConfigurations.Mapping
{
    public class ValueMapping
    {
        /// <summary>
        /// Creates a ValueMapping object that is case insenitive and has a mapping type of Any
        /// </summary>
        /// <param name="mapFromValue"></param>
        /// <param name="mapToValue"></param>
        public ValueMapping(string mapFromValue, string mapToValue) :this(mapFromValue, mapToValue, false, MappingType.Any) { }
        /// <summary>
        /// Creates a ValueMapping object that has a mapping type of Any
        /// </summary>
        /// <param name="mapFromValue"></param>
        /// <param name="mapToValue"></param>
        /// <param name="canSensitive"></param>
        public ValueMapping(string mapFromValue, string mapToValue, bool canSensitive) : this(mapFromValue, mapToValue, canSensitive, MappingType.Any) { }
        /// <summary>
        /// Creates a ValueMapping object.
        /// </summary>
        /// <param name="mapFromValue"></param>
        /// <param name="mapToValue"></param>
        /// <param name="canSensitive"></param>
        /// <param name="mappingType"></param>
        public ValueMapping(string mapFromValue, string mapToValue, bool canSensitive, MappingType mappingType)
        {
            MapFromValue = mapFromValue;
            MapToValue = mapToValue;
            IsCanSensitive = canSensitive;
            MappingType = mappingType;
        }

        public MappingType MappingType { get; set; }
        public bool IsCanSensitive { get; set; }
        public string MapFromValue { get; set; }
        public string MapToValue { get; set; }
    }
}
