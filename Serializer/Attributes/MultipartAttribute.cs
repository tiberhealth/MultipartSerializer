using System;

namespace TiberHealth.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class MultipartAttribute: Attribute
    {
        /// <summary>
        /// Name override for the property.
        /// This names take rank over the JSON names and the property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If applied to property that is a class, the field that should be used as a file.
        /// If this is not set, the property is serialized. 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Identifies that an enum value should be represented by its string not number
        /// (default is false)
        /// </summary>
        public bool EnumAsString { get; set; }

        /// <summary>
        /// Tells the serializer to create a string content with all the values in a delimited string format
        /// </summary>
        public bool EnumerationAsXsv { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EnumerationDelimiter { get; set; }

        /// <summary>
        /// Main Constructor
        /// </summary>
        public MultipartAttribute()
        {
            this.EnumerationAsXsv = false;
            this.EnumerationDelimiter = ",";
            this.EnumAsString = false;
        }

        /// <summary>
        /// Constructor to set he name field
        /// </summary>
        /// <param name="name">String value for the name of the part (null defaults to the property name)</param>
        public MultipartAttribute(string name): this()
        {
            this.Name = name;
        }
    }
}
