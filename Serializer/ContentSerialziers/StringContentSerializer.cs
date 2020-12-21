using System;
using System.Net.Http;
using System.Reflection;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal sealed class StringContentSerializer: ContentSerializerBase
    {
        public StringContentSerializer(PropertyInfo propertyInfo, object bodyObject, MultipartAttribute attribute = null)
            : base(propertyInfo, bodyObject, attribute)
        {
        }

        protected override HttpContent Content()
        {
            if (this.Property.PropertyType.IsEnum) return new EnumContentSerializer(this.Property, this.BodyObject).ToContent();
            return new StringContent(this.PropertyValue.ToString());
        }
    }
}
