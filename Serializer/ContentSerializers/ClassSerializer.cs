namespace TiberHealth.Serializer.ContentSerializers
{
    internal class ClassSerializer<TValue> : ContentSerializer<TValue>
    {
        internal ClassSerializer(TValue value, ISerializerOptions serializerOptions) : base(value, null, null, serializerOptions)
        {
        }

        internal ClassSerializer(TValue value, IContentSerializer parent, PropertyInfo property, ISerializerOptions serializerOptions)
            : base(value, parent, property, serializerOptions)
        {
        }

        protected override HttpContent[] Content() =>
            this.IsFile(out var attribute)
                ? this.SerializeFile(attribute)
                : this.Properties
                    .Where(prop => prop.IsNotIgnore())
                    .SelectMany(this.SerializeProperty)
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
            if (property == null) return Array.Empty<HttpContent>();
            var propValue = property.GetValue(this.Value);

            if (propValue == null) return Array.Empty<HttpContent>();

            var serializerType = typeof(ContentSerializer<>).MakeGenericType(new[] {propValue.GetType()});
            var serializer = Activator.CreateInstance(serializerType, propValue, this, property, this.SerializerOptions) as ISerializer;
            return serializer?.ToContent() ?? Array.Empty<HttpContent>();
        }

        private HttpContent[] SerializeFile(MultipartFileAttribute attribute)
        {
            var byteArray = attribute.GetFile(this.Value);
            var fileContents = new ByteArraySerializer(byteArray, this.GetParent(), this.Property, this.SerializerOptions).ToContent();

            attribute.SetDisposition(this.Value, fileContents.SingleOrDefault()?.Headers);
            return fileContents;
        }

    }
}
