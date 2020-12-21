using System;
using System.Net.Http;
using System.Reflection;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal abstract class ContentSerializerBase : SerializerBase
    {
        protected PropertyInfo Property { get; }
        protected object PropertyValue { get; }
        protected MultipartAttribute OverrideAttribute { get; }

        protected virtual bool DefaultDispositionHeader => true;

        protected bool HasAttribute => this.OverrideAttribute != null;

        protected ContentSerializerBase(PropertyInfo property, object bodyObject, Attribute attribute) : base(bodyObject)
        {
            this.Property = property;
            this.OverrideAttribute = attribute as MultipartAttribute;

            this.PropertyValue = this.GetValue();
        }

        protected string PartName => this.OverrideAttribute?.GetName(this.BodyObject) ?? this.Property.MultipartName();
        protected object GetValue() => this.OverrideAttribute?.GetValue(this.BodyObject) ??
                                        (this.HasAttribute ? this.BodyObject : this.Property.GetValue(this.BodyObject));

        protected HttpContent SetDispositionHeader(HttpContent content)
        {
            content.Headers.ContentDisposition ??= new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = $"\"{ this.PartName }\""
            };

            if (this.HasAttribute && (this.OverrideAttribute is MultipartFileAttribute multipartFileAttribute))
            {
                multipartFileAttribute.SetDisposition(this.BodyObject, content.Headers);
            }

            return content;
        }

        public override HttpContent ToContent() =>
            this.PropertyValue == null ?
            null :
            this.DefaultDispositionHeader ? this.SetDispositionHeader(this.Content()) : this.Content();
    }
}
