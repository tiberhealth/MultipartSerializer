using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json.Serialization;
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

        protected IEnumerable<PropertyInfo> Properties => this.GetProperties(this.PropertyType);

        private IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            var properties = new Dictionary<string, PropertyInfo>();
            this.GetProperties(properties, type);
            return properties.OrderBy(item => item.Key).Select(item => item.Value).Where(item => item != null).ToArray();
        }

        private void GetProperties(Dictionary<string, PropertyInfo> activeProperties, Type type)
        {
            foreach (var property in type.GetProperties())
            {
                if (activeProperties.ContainsKey(property.Name)) {
                    this.SwapProperties(activeProperties, property);
                    continue;
                }

                activeProperties.Add(property.Name, property);
            }

            type.GetInterfaces().ToList().ForEach(item => this.GetProperties(activeProperties, item));                
        }

        private void SwapProperties(Dictionary<string, PropertyInfo> properties, PropertyInfo property)
        {
            if (property == null) return;
            if (!properties.TryGetValue(property.Name, out var current)) throw new KeyNotFoundException($"key {property.Name} told to swap, but not found.");

            var currentCustom = current.GetCustomAttributes();
            var propertyCustom = property.GetCustomAttributes();

            if (currentCustom.Count() == 0 && propertyCustom.Count() == 0) return;
            if (currentCustom.Count() > 0) return;
            if (propertyCustom.Count() == 0) return;

            properties[property.Name] = property;           
        }

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
