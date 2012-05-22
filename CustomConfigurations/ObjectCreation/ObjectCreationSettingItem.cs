using System;

namespace CustomConfigurations.ObjectCreation
{
    public class ObjectCreationSettingItem : IComparable<ObjectCreationSettingItem>
    {
        public string OriginalName { get; set; }
        public string MapToName { get; set; }
        public string DefaultValue { get; set; }
        public string OriginalValue { get; set; }
        public ObjectCreationSettingType CreationSettingType { get; set; }

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(MapToName.Trim()))
                    return MapToName;

                return OriginalName;
            }
        }

        public string Value
        {
            get
            {
                if (!string.IsNullOrEmpty(OriginalValue))
                    return OriginalValue;

                return DefaultValue;
            }
        }


        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        ///                     Value 
        ///                     Meaning 
        ///                     Less than zero 
        ///                     This object is less than the <paramref name="other"/> parameter.
        ///                     Zero 
        ///                     This object is equal to <paramref name="other"/>. 
        ///                     Greater than zero 
        ///                     This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public int CompareTo(ObjectCreationSettingItem other)
        {
            return Equals(other) ? 0 : 1;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ObjectCreationSettingItem)) return false;
            return Equals((ObjectCreationSettingItem)obj);
        }

        public bool Equals(ObjectCreationSettingItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.OriginalName, OriginalName) && Equals(other.MapToName, MapToName) && Equals(other.CreationSettingType, CreationSettingType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (OriginalName != null ? OriginalName.GetHashCode() : 0);
                result = (result * 397) ^ (MapToName != null ? MapToName.GetHashCode() : 0);
                result = (result * 397) ^ CreationSettingType.GetHashCode();
                return result;
            }
        }
    }
}
