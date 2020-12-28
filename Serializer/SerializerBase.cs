using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using TiberHealth.Serializer.Attributes;
using TiberHealth.Serializer.Extensions;

namespace TiberHealth.Serializer
{
    public abstract class SerializerBase<TObject> : ISerializer
    {
        protected TObject Value { get; }
        protected abstract HttpContent[] Content();

        protected SerializerBase(TObject value) => this.Value = value;

        public virtual HttpContent[] ToContent() => this.Content()?.Where(item => item != null).ToArray();

        protected Type PropertyType => typeof(TObject);

        protected bool IsClass => this.PropertyType.IsClass;
        protected bool IsEnum => this.PropertyType.IsEnum;
        protected IEnumerable<PropertyInfo> Properties => this.PropertyType.GetProperties();

        private MultipartAttribute MultipartAttribute =>
            this.PropertyType
                .GetCustomAttributes()
                .OfType<MultipartAttribute>()
                .SingleOrDefault();

        private TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute =>
            this.PropertyType.GetCustomAttribute<TAttribute>();

        protected bool IsType<TType>() => this.PropertyType.IsType<TType>();
        protected bool IsNotType<TType>() => !this.IsType<TType>();
        
        protected string DetermineName(PropertyInfo property)
        {
            return
                property?.MultipartName(() =>
                    this.MultipartAttribute?.Name ??
                    this.GetAttribute<Newtonsoft.Json.JsonPropertyAttribute>()?.PropertyName ??
                    this.GetAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>()?.Name
                ) ?? property?.Name;
        }
    }
}