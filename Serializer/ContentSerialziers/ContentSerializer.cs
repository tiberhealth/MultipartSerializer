using System.Net.Http;
using System.Reflection;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class ContentSerializer : ContentSerializerBase
    {
        protected override bool DefaultDispositionHeader => false;

        public ContentSerializer(PropertyInfo propertyInfo, object bodyObject, MultipartAttribute attribute = null) : base(propertyInfo, bodyObject, attribute)
        {
        }

        protected override HttpContent Content() =>
            this.Property.PropertyType.FullName switch
            {
                "System.Byte[]" => new ByteArrayContentSerializer(this.Property, this.BodyObject, this.OverrideAttribute).ToContent(),
                "System.String" => new StringContentSerializer(this.Property, this.BodyObject, this.OverrideAttribute).ToContent(),
                _ => this.Property.PropertyType.IsClass ?
                        new ClassContentSerializer(this.Property, this.BodyObject, this.OverrideAttribute).ToContent() :
                        new StringContentSerializer(this.Property, this.BodyObject, this.OverrideAttribute).ToContent()
            };
    }
}
