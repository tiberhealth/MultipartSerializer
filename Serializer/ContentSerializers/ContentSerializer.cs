﻿using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using TiberHealth.Serializer.Extensions;

namespace TiberHealth.Serializer.ContentSerializers
{
    internal class ContentSerializer<TValue> : SerializerBase<TValue>, IContentSerializer
    {
        private IContentSerializer ParentObject { get; }
        protected PropertyInfo Property { get; }

        public string Name
        {
            get
            {
                if (this.Property == null) return null;

                var parentName = this.ParentObject?.Name?.Trim(); // Trims might not be necessarry.
                var propertyName = this.DetermineName(this.Property)?.Trim();

                if (this.Property.PropertyType.IsEnumerable()) propertyName += "[]";

                return !string.IsNullOrWhiteSpace(parentName) ? $"{parentName}[{propertyName}]" : propertyName;
            }
        }

        protected IContentSerializer GetParent() => this.ParentObject;

        // ReSharper disable once MemberCanBeProtected.Global Justification: Used in Activator.Create.
        public ContentSerializer(TValue value, IContentSerializer parentObject, PropertyInfo property) : base(value)
        {
            this.Property = property;
            this.ParentObject = parentObject;
        }

        protected override HttpContent[] Content()
        {
            //TODO: Turn this into a proper factory, if possible
            if (this.IsEnum)
                return new EnumSerializer<TValue>(this.Value, this.ParentObject, this.Property).ToContent();

            if (this.IsType<byte[]>())
                return new ByteArraySerializer(this.Value as byte[], this.ParentObject, this.Property).ToContent();

            if (this.IsType<string>() || this.PropertyType.IsValueType)
                return new StringSerializer(this.Value.ToString(), this.ParentObject, this.Property).ToContent();

            if (this.IsType<IEnumerable>())
                return new EnumerableSerializer(this.Value as IEnumerable, this.ParentObject, this.Property)
                    .ToContent();

            return this.IsClass
                ? new ClassSerializer<TValue>(this.Value, this.ParentObject, this.Property).ToContent()
                : null;
        }

        protected HttpContent Content<TContent>(object valueOverride = null)
            where TContent : HttpContent =>
            this.SetHeaders(Activator.CreateInstance(typeof(TContent), valueOverride ?? this.Value) as HttpContent);

        protected virtual HttpContent SetHeaders(HttpContent content)
        {
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = $"\"{this.Name}\""
            };

            return content;
        }
    }
}