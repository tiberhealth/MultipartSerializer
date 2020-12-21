using System;
using System.Net.Http;
using System.Reflection;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal sealed class EnumContentSerializer : ContentSerializerBase
    {
        public EnumContentSerializer(PropertyInfo propertyInfo, object bodyObject, MultipartAttribute attribute = null) : base(propertyInfo, bodyObject, attribute)
        { }

        protected override HttpContent Content()
        {
            var enumValue = (int)this.PropertyValue;
            return new StringContent(enumValue.ToString());
        }
    }
}
