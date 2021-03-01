using System;
namespace TiberHealth.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumSerializedValueAttribute: Attribute
    {
        public object Value { get; set; }
        public EnumSerializedValueAttribute(object value) => this.Value = value;
    }
}
