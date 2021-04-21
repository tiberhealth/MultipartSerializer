using System;
namespace TiberHealth.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class EnumSerializedValueAttribute: Attribute, IMultipartAttribute
    {
        public object Value { get; set; }
        public EnumSerializedValueAttribute(object value) => this.Value = value;
    }
}
