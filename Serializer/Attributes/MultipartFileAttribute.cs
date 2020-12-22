using System;
using System.Net.Http.Headers;
using TiberHealth.Serializer.Exceptions;

namespace TiberHealth.Serializer
{
    /// <summary>
    /// Identifies a class object that is a file. Class objects that are not identified 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MultipartFileAttribute: MultipartAttribute
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }

        /// <summary>
        /// Main constructor
        /// </summary>
        public MultipartFileAttribute(): this(null)
        {}

        /// <summary>
        /// constructor to set the name of the part
        /// </summary>
        /// <param name="name">String value to use as the name. (Null means to use the prop[erty name</param>
        public MultipartFileAttribute(string name): base(name)
        {
            this.Value = "FileBytes";  // Default for value
        }

        /// <summary>
        /// Gets the value of the property that is defined in the attribute Value field
        /// If the property does not exist, then the value for the Name property is passed as a constant.
        /// </summary>
        /// <param name="bodyObject">Object to look for the name object</param>
        /// <returns>The value for Value</returns>
        internal override object GetValue(object bodyObject)
        {
            if (string.IsNullOrEmpty(this.Value)) throw new ArgumentNullException(nameof(this.Value), "Byte[] field required for file part.");

            var objectValue = base.GetValue(bodyObject);
            if (objectValue is byte[]) return objectValue;

            throw new InvalidTypeException("Files must be presented as a byte array");            
        }

        internal void SetDisposition(object propertyValue,  HttpContentHeaders headers)
        {
            if (headers.ContentDisposition == null) return;

            this.SetFileName(propertyValue, headers.ContentDisposition);
            this.SetContentType(propertyValue, headers);
        }

        private void SetFileName(object propertyValue, ContentDispositionHeaderValue contentDisposition ) =>
            contentDisposition.FileName ??= this.GetField(this.FileName ?? nameof(this.FileName), propertyValue);

        private void SetContentType(object propertyValue, HttpContentHeaders headers)
        {
            var mimeType = this.GetField(this.ContentType ?? nameof(this.ContentType), propertyValue, !string.IsNullOrWhiteSpace(this.ContentType));
            if (string.IsNullOrWhiteSpace(mimeType)) return;

            try
            {
                headers.ContentType ??= new MediaTypeHeaderValue(mimeType);
            }
            catch (Exception ex)
            {
                throw new InvalidTypeException($"Content Type {mimeType} is invalid.", ex);
            }
        }


        private string GetField(string fieldName, object body, bool defaultValue = true) => 
            body.GetType().GetProperty(fieldName)?.GetValue(body)?.ToString() ?? (defaultValue ? fieldName : null);

    }
}
