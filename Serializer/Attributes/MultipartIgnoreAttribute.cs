﻿namespace TiberHealth.Serializer.Attributes
{
    /// <summary>
    /// Set a property to be ignored by the serializer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MultipartIgnoreAttribute : Attribute, IMultipartAttribute
    {    }
}
