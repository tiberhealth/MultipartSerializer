namespace TiberHealth.Serializer.Attributes
{
    /// <summary>
    /// Identifies a class object that is a file. Class objects that are not identified 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MultipartFileAttribute: MultipartAttribute, IMultipartAttribute
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
        // ReSharper disable once MemberCanBePrivate.Global
        public MultipartFileAttribute(string name): base(name)
        {
            this.Value = "FileBytes";  // Default for value
        }

        public byte[] GetFile<TSource>(TSource source)
        {
            var byteProperty = source.GetType().GetProperty(this.Value ?? "FileBytes");
            if (byteProperty == null) return new byte[0];

            var value = byteProperty.GetValue(source) as byte[];
            return value ?? new byte[0];
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

        private string GetField(string fieldName, object body, bool defaultValue = true)
        {
            var fieldProperty = body.GetType().GetProperty(fieldName);
            if (fieldProperty == null)
            {
                return defaultValue ? fieldName : null;
            }
            
            return fieldProperty.GetValue(body)?.ToString();
        }
    }
}
