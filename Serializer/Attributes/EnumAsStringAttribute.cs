using System;
namespace TiberHealth.Serializer.Attributes
{
    public class EnumAsStringAttribute: Attribute
    {
        public bool Enabled { get; set; }
        public EnumAsStringAttribute(bool enabled = true) => this.Enabled = enabled;
    }
}
