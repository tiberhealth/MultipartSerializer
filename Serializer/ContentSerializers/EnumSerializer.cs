using System;
using System.Net.Http;
using System.Reflection;

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
                return new[] { this.Content<StringContent>(((int)enumValue).ToString()) };
            }

            return null;
        }
    }
}
