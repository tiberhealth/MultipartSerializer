using System;
using System.Net.Http;
using System.Reflection;
using TiberHealth.Serializer.Attributes;
using TiberHealth.Serializer.Extensions;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class EnumSerializer<TEnum>: ContentSerializer<TEnum>
    {
        public EnumSerializer(TEnum value, IContentSerializer parent, PropertyInfo property): base(value, parent, property)
        {
        }

        protected override HttpContent[] Content()
        {
            if (Enum.TryParse(typeof(TEnum), this.Value.ToString(), out var enumValue)) {

                var attribute = this.Property.HasCustomAttribute<MultipartAttribute>(out var propAttribute) ? propAttribute : 
                    typeof(TEnum).HasCustomAttribute<MultipartAttribute>(out var enumAttribute) ? enumAttribute :
                    null;

                return (attribute?.EnumAsString ?? false) ?
                    new[] { this.Content<StringContent>(enumValue.ToString()) } :
                    new[] { this.Content<StringContent>(((int)enumValue).ToString()) };
            }

            return null;
        }
    }
}
