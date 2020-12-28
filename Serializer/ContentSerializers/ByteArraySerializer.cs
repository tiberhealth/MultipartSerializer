using System.Net.Http;
using System.Reflection;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class ByteArraySerializer: ContentSerializer<byte[]>
    {
        internal ByteArraySerializer(byte[] value, IContentSerializer parent, PropertyInfo property): base(value, parent, property)
        {
        }

        protected override HttpContent[] Content() => new[] { this.Content<ByteArrayContent>() };
    }
}
