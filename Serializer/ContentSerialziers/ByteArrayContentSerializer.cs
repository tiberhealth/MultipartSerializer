using System;
using System.Net.Http;
using System.Reflection;
using TiberHealth.Serializer.Exceptions;

namespace TiberHealth.Serializer.ContentSerializers
{
    public class ByteArrayContentSerializer : ContentSerializerBase
    {
        public ByteArrayContentSerializer(PropertyInfo propertyInfo, object bodyObject, MultipartAttribute attribute = null) : base(propertyInfo, bodyObject, attribute)
        {
            if (this.PropertyValue == null) return;
            if (this.PropertyValue is byte[]) return;

            throw new InvalidTypeException($"Property {this.Property.Name} is marked as a byte array, but the contents are not a byte array");
        }

        protected override HttpContent Content() =>
            (!(this.PropertyValue is byte[] array) || array.Length == 0) ? null : new ByteArrayContent(array, 0, array.Length);

    }
}
