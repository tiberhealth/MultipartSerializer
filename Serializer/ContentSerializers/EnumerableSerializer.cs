using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class EnumerableSerializer: ContentSerializer<IEnumerable>
    {
        public EnumerableSerializer(IEnumerable value, IContentSerializer parentObject, PropertyInfo property) : base(value, parentObject, property)
        {
        }

        protected override HttpContent[] Content()
        {
            var content = new List<HttpContent>();
            foreach (var item in this.Value)
            {
                var serializerType = typeof(ContentSerializer<>).MakeGenericType(new[] {item.GetType()});
                var serializer = Activator.CreateInstance(serializerType, item, this.GetParent(), this.Property) as ISerializer;
                content.AddRange(serializer?.ToContent() ?? Array.Empty<HttpContent>());
            }
            return content.Where(item => item != null).ToArray();
        }
    }
}
