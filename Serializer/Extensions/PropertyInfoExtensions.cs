using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TiberHealth.Serializer.Extensions
{
    public static class PropertyInfoExtensions
    {
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
            property.GetCustomAttributes().Any(item =>
                new[]
                {
                    typeof(TiberHealth.Serializer.MultipartIgnoreAttribute),
                    typeof(Newtonsoft.Json.JsonIgnoreAttribute),
                    typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)
                }.Contains(item.GetType())
            );

        /// <summary>
        /// Identifies an object elment if it should NOT be ignored.
        /// <seealso cref="IsIgnore(PropertyInfo)"/>
        /// </summary>
        /// <param name="property">Property to Analyze</param>
        /// <returns>Boolean: True => Ignore flag not set (Do NOT Ignore), False => Ignore flag is set (Ignore)</returns>
        internal static bool IsNotIgnore(this PropertyInfo property) => !property.IsIgnore();

        /// <summary>
        /// Gets the name for the Multipart contexst
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static string MultipartName(this PropertyInfo property) =>
            (
                property.HasCustomAttribute<TiberHealth.Serializer.MultipartAttribute>(out var multipartAttribute) ? multipartAttribute.Name :
                property.HasCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>(out var newtonsoftAttribute) ? newtonsoftAttribute.PropertyName : 
                property.HasCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>(out var systemJsonAttribute) ? systemJsonAttribute.Name :
                null
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
        /// <typeparam name="TAttirbute">Attribute to find</typeparam>
        /// <param name="property">The property to check</param> 
        /// <param name="attribute">Out variable of the actual attribute</param>
        /// <returns>True/False indcating if the property was found</returns>
        public static bool HasCustomAttribute<TAttribute>(this PropertyInfo property, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = property.GetCustomAttribute<TAttribute>();
            return attribute != null;
        }

        public static bool HasCustomerAttribute<TAttribute>(this Type type, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = type.GetCustomAttribute<TAttribute>();
            return attribute != null; 
        }
    }
}
