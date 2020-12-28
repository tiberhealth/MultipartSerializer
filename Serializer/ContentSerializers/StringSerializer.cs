using System.Net.Http;
using System.Reflection;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class StringSerializer : ContentSerializer<string>
    {
        internal StringSerializer(string value, IContentSerializer parent, PropertyInfo property) : base(value, parent, property)
        {
        }

        protected override HttpContent[] Content() =>
            new[] { this.Content<StringContent>() };
    }
}