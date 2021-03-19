using System;
namespace TiberHealth.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class EnumAsStringAttribute: Attribute
    {
        public bool Enabled { get; set; }

        public EnumAsStringAttribute() => this.Enabled = true; // Default as truthy
        public EnumAsStringAttribute(bool enabled) : this() => this.Enabled = enabled;
    }
}
