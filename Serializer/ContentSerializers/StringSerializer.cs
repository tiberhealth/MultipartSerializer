using System.Net.Http;
using System.Reflection;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class StringSerializer : ContentSerializer<string>
    {
        internal StringSerializer(string value, IContentSerializer parent, PropertyInfo property, ISerializerOptions serializerOptions) :
            base(value, parent, property, serializerOptions)
        {
        }

        protected override HttpContent[] Content() =>
            new[] { this.Content<StringContent>() };
    }
}