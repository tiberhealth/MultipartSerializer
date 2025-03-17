namespace TiberHealth.Serializer.ContentSerializers
{
    internal class ByteArraySerializer: ContentSerializer<byte[]>
    {
        internal ByteArraySerializer(byte[] value, IContentSerializer parent, PropertyInfo property, ISerializerOptions serializerOptions)
            : base(value, parent, property, serializerOptions)
        {
        }

        protected override HttpContent[] Content() => new[] { this.Content<ByteArrayContent>() };
    }
}
