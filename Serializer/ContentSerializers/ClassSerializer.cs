using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
 using TiberHealth.Serializer.Attributes;
using TiberHealth.Serializer.Extensions;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class ClassSerializer<TValue> : ContentSerializer<TValue>
    {
        internal ClassSerializer(TValue value) : base(value, null, null)
        {
        }

        internal ClassSerializer(TValue value, IContentSerializer parent, PropertyInfo property) : base(value, parent,
            property)
        {
        }

        protected override HttpContent[] Content() =>
            this.IsFile(out var attribute)
                ? this.SerializeFile(attribute)
                : this.Properties
                    .Where(prop => prop.IsNotIgnore())
                    .SelectMany(SerializeProperty)
                    .ToArray();

        private bool IsFile(out MultipartFileAttribute attribute)
        {
            if (this.Property != null)
                return this.Property.HasCustomAttribute(out attribute) ||
                       this.PropertyType.HasCustomAttribute(out attribute) ||
                       this.Property.PropertyType.HasCustomAttribute(out attribute);
            
            attribute = null;
            return false;
        }

        private HttpContent[] SerializeProperty(PropertyInfo property)
        {
            var propValue = property.GetValue(this.Value);

            var serializerType = typeof(ContentSerializer<>).MakeGenericType(new[] {propValue.GetType()});
            var serializer = Activator.CreateInstance(serializerType, propValue, this, property) as ISerializer;
            return serializer?.ToContent() ?? Array.Empty<HttpContent>();
        }

        private HttpContent[] SerializeFile(MultipartFileAttribute attribute)
        {
            var byteArray = attribute.GetFile(this.Value);
            var fileContents = new ByteArraySerializer(byteArray, this.GetParent(), this.Property).ToContent();

            attribute.SetDisposition(this.Value, fileContents.SingleOrDefault()?.Headers);
            return fileContents;
        }

    }
}