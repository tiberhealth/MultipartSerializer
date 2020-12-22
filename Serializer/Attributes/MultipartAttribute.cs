using System;

namespace TiberHealth.Serializer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
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
        /// Main Constructor
        /// </summary>
        public MultipartAttribute() { }

        /// <summary>
        /// Constructor to set he name field
        /// </summary>
        /// <param name="name">String value for the name of the part (null defaults to the property name)</param>
        public MultipartAttribute(string name): this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Get the name of the Property that is marked to be the Name field
        /// If the property does not exist, assume that it is a static value
        /// </summary>
        /// <param name="bodyObject">Object to look for the name object</param>
        /// <returns>The value for Name</returns>
        internal string GetName(object bodyObject)
        {
            if (string.IsNullOrWhiteSpace(this.Name)) return null;
            var property = bodyObject.GetType().GetProperty(Name);

            return property?.GetValue(bodyObject)?.ToString() ?? this.Name;
        }

        /// <summary>
        /// Gets the value of the property that is defined in the attribute Value field
        /// If the property does not exist, then the value for the Name property is passed as a constant.
        /// </summary>
        /// <param name="bodyObject">Object to look for the name object</param>
        /// <returns>The value for Value</returns>
        internal virtual object GetValue(object bodyObject)
        {
            if (string.IsNullOrWhiteSpace(this.Value)) return null;
            var property = bodyObject.GetType().GetProperty(this.Value);

            return property?.GetValue(bodyObject) ?? this.Value;
        }
    }
}
