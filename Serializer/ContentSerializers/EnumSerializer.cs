using System;
using System.Net.Http;
using System.Reflection;
using TiberHealth.Serializer.Attributes;
using TiberHealth.Serializer.Extensions;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class EnumSerializer<TEnum>: ContentSerializer<TEnum>
    {
        public EnumSerializer(TEnum value, IContentSerializer parent, PropertyInfo property, ISerializerOptions serializerOptions)
            : base(value, parent, property, serializerOptions)
        {
        }

        protected override HttpContent[] Content()
        {
            if (Enum.TryParse(typeof(TEnum), this.Value.ToString(), out var enumValue))
            {
                var attribute = this.Property.HasCustomAttribute<MultipartAttribute>(out var propAttribute) ? propAttribute : 
                    typeof(TEnum).HasCustomAttribute<MultipartAttribute>(out var enumAttribute) ? enumAttribute :
                    null;

                var enumAsString = this.Property.HasCustomAttribute<EnumAsStringAttribute>(out var easAttribute) ? easAttribute :
                    typeof(TEnum).HasCustomAttribute<EnumAsStringAttribute>(out var easEnumAttribute) ? easEnumAttribute :
                    null;

                if (
                    (attribute?.EnumAsString ??  false) ||
                    (enumAsString?.Enabled ?? false))
                {
                    var field = enumValue.GetType().GetField(enumValue.ToString());
                    field.HasCustomAttribute<EnumSerializedValueAttribute>(out var customAttribute);

                    var serializedValue = customAttribute?.Value?.ToString() ??
                                                this.SerializerOptions?.EnumNameResolver?.ConvertName(enumValue.ToString()) ??
                                                enumValue.ToString();

                    return new[] { this.Content<StringContent>(serializedValue) };
                }

                return new[] { this.Content<StringContent>(((int)enumValue).ToString()) };
            }

            return null;
        }
    }
}
