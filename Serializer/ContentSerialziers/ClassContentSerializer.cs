using System;
using System.Net.Http;
using System.Reflection;
using TiberHealth.Serializer.Exceptions;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class ClassContentSerializer : ContentSerializerBase
    {
        public ClassContentSerializer(PropertyInfo propertyInfo, object bodyObject, MultipartAttribute attribute) : base(propertyInfo, bodyObject, attribute)
        {
        }

        protected override HttpContent Content()
        {
            if (!this.Property.PropertyType.IsClass) throw new InvalidTypeException($"Property {this.Property.Name} is not a class, but in the Class serializer");

            if (this.Property.HasCustomAttribute<MultipartFileAttribute>(out var propertyAttribute)) return this.Content(propertyAttribute);
            if (this.Property.DeclaringType.HasCustomerAttribute<MultipartFileAttribute>(out var typeAttribute)) return this.Content(typeAttribute);
            if (this.Property.HasCustomAttribute<MultipartAttribute>(out var multipartAttribute)) return Content(multipartAttribute);

            return DefaultContent();
        }

        private HttpContent Content(MultipartFileAttribute attribute) => new ByteArrayContentSerializer(this.Property, this.PropertyValue, attribute).ToContent();
        private HttpContent Content(MultipartAttribute attribute) => new ContentSerializer(this.Property, this.PropertyValue, attribute).ToContent();

        private HttpContent DefaultContent()
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(this.BodyObject);
                return new StringContent(json);
            }
            catch (Exception ex)
            {
                throw new GenericSerializationException($"Unable to serialize object {this.Property.Name} to JSON.", ex);
            }
        }
    }
}
