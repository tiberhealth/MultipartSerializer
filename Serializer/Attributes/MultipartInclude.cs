namespace TiberHealth.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MultipartInclude: Attribute, IMultipartAttribute
    { }
}
