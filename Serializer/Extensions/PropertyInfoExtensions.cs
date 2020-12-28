using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TiberHealth.Serializer.Attributes;

namespace TiberHealth.Serializer.Extensions
{
    public static class PropertyInfoExtensions
    {

        private static readonly Type[] IgnoreAttributes =
            new[]
                {
                    typeof(MultipartIgnoreAttribute),
                    typeof(Newtonsoft.Json.JsonIgnoreAttribute),
                    typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)
                };

        private static readonly Type[] IncludeAttributes =
            new[]
                {
                    typeof(MultipartInclude),
                    typeof(MultipartAttribute)
                };

        /// <summary>
        /// Identifies an object element if it should be ignored.
        /// To have a property ignored, attach one of the following attributes to the property.
        /// <seealso cref="MultipartIgnoreAttribute"/>
        /// <seealso cref="Newtonsoft.Json.JsonIgnoreAttribute"/>
        /// <seealso cref="System.Text.Json.Serialization.JsonIgnoreAttribute"/>
        /// </summary>
        /// <param name="property">Property to Analyze</param>
        /// <returns>Boolean: True => Ignore flag is set (ignore), False => Ignore flag is not set (Do NOT ignore)</returns>
        internal static bool IsIgnore(this PropertyInfo property) =>
            !property.GetCustomAttributes().Any(item => IncludeAttributes.Contains(item.GetType())) &&
            property.GetCustomAttributes().Any(item => IgnoreAttributes.Contains(item.GetType()));

        /// <summary>
        /// Identifies an object elment if it should NOT be ignored.
        /// <seealso cref="IsIgnore(PropertyInfo)"/>
        /// </summary>
        /// <param name="property">Property to Analyze</param>
        /// <returns>Boolean: True => Ignore flag not set (Do NOT Ignore), False => Ignore flag is set (Ignore)</returns>
        internal static bool IsNotIgnore(this PropertyInfo property) => !property.IsIgnore();

        /// <summary>
        /// identify if the property is enumerable, and not a string
        /// </summary>
        /// <param name="property">Property to check for an enumerable</param>
        /// <returns>True if the property is enumerable false if not</returns>
        internal static bool IsEnumerable(this PropertyInfo property) =>
            typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && typeof(string) != property.PropertyType;

        /// <summary>
        /// Gets the name for the Multipart context
        /// </summary>
        /// <param name="property">Property to check for Multipart Attribute</param>
        /// <param name="defaultFactory">Factory to build default value (optional)</param>
        /// <returns>The determined name</returns>
        internal static string MultipartName(this PropertyInfo property, Func<string> defaultFactory = null) =>
            (
                property.HasCustomAttribute<MultipartAttribute>(out var multipartAttribute) ? multipartAttribute.Name :
                property.HasCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>(out var newtonsoftAttribute) ? newtonsoftAttribute.PropertyName : 
                property.HasCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>(out var systemJsonAttribute) ? systemJsonAttribute.Name :
                defaultFactory?.Invoke()
            ) ?? property.Name;

        /// <summary>
        /// Checks the property for a custom attribute
        /// </summary>
        /// <typeparam name="TAttribute">Attribute to find</typeparam>
        /// <param name="property">The property to check</param>
        /// <returns>True/False indicating if the property was found</returns>
        public static bool HasCustomAttribute<TAttribute>(this PropertyInfo property) where TAttribute: Attribute => property.HasCustomAttribute<TAttribute>(out _);

        /// <summary>
        /// Checks the property for a custom attribute
        /// </summary>
        /// <typeparam name="TAttribute">Attribute to find</typeparam>
        /// <param name="property">The property to check</param> 
        /// <param name="attribute">Out variable of the actual attribute</param>
        /// <returns>True/False indicating if the property was found</returns>
        public static bool HasCustomAttribute<TAttribute>(this PropertyInfo property, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = property?.GetCustomAttribute<TAttribute>();
            return attribute != null;
        }

        /// <summary>
        /// Determines if a type has a custom attribute
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="attribute">(Out) the resulting instantiated attribute class that was found</param>
        /// <typeparam name="TAttribute">Attribute type that is being checked for in the type</typeparam>
        /// <returns>True/False if the type contained the attribute</returns>
        public static bool HasCustomAttribute<TAttribute>(this Type type, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = type.GetCustomAttribute<TAttribute>();
            return attribute != null; 
        }
        
        /// <summary>
        /// Determine if a type is a specific type
        /// </summary>
        /// <param name="type">Type object to check</param>
        /// <typeparam name="TType">Type expecting</typeparam>
        /// <returns></returns>
        public static bool IsType<TType>(this Type type) =>
            (type == typeof(TType)) || typeof(TType).IsAssignableFrom(type);

        /// <summary>
        /// Determine if a type IS NOT a specific type
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <typeparam name="TType">Type expecting</typeparam>
        /// <returns></returns>
        public static bool IsNotType<TType>(this Type type) => !type.IsType<TType>();

        public static bool IsEnumerable(this Type type) =>
            type.IsType<IEnumerable>() &&
            type.IsNotType<byte[]>() &&
            type.IsNotType<string>();
    }
}
