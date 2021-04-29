    using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using TiberHealth.Serializer.Attributes;
using TiberHealth.Serializer.Extensions;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class EnumerableSerializer : ContentSerializer<IEnumerable>
    {
        public override string Name
        {
            get
            {
                var baseName = base.Name;
                var attribute = this.Property.HasCustomAttribute<MultipartAttribute>(out var propAttribute) ? propAttribute :
                    null;

                return (attribute?.EnumerationAsXsv ?? false) ?
                    baseName.Trim().TrimEnd('[', ']') :
                    baseName;
            }
        }
        public EnumerableSerializer(IEnumerable value, IContentSerializer parentObject, PropertyInfo property, ISerializerOptions serializerOptions)
            : base(value, parentObject, property, serializerOptions)
        {
        }

        protected override HttpContent[] Content()
        {
            var contentSections = new List<HttpContent>();
            foreach (var item in this.Value)
            {
                var serializerType = typeof(ContentSerializer<>).MakeGenericType(new[] { item.GetType() });
                var serializer = Activator.CreateInstance(serializerType, item, this.GetParent(), this.Property, this.SerializerOptions) as ISerializer;
                contentSections.AddRange(serializer?.ToContent() ?? Array.Empty<HttpContent>());
            }

            var attribute = this.Property.HasCustomAttribute<MultipartAttribute>(out var propAttribute) ? propAttribute : null;

            return (attribute?.EnumerationAsXsv ?? false) ?
                new[] { ContentAsString(contentSections, attribute) } :
                contentSections.Where(item => item != null).ToArray();

        }

        private HttpContent ContentAsString(IEnumerable<HttpContent> contentSections, MultipartAttribute attribute)
        {
            var delmiter = attribute.EnumerationDelimiter?.Trim() ?? ",";

            var valueList = new List<string>();
            foreach (var section in contentSections)
            {
                var stringTask = section.ReadAsStringAsync();
                stringTask.Wait();

                valueList.Add(stringTask.Result);
            }

            return this.Content<StringContent>(string.Join(delmiter, valueList.Where(item => item != null).ToArray()));
        }
    }
}
