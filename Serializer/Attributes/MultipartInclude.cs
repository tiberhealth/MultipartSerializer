using System;
namespace TiberHealth.Serializer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MultipartInclude: Attribute
    {}
}
