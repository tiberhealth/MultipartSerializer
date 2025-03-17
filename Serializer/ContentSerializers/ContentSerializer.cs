namespace TiberHealth.Serializer.ContentSerializers
{
    internal class ContentSerializer<TValue> : SerializerBase<TValue>, IContentSerializer
    {
        private IContentSerializer ParentObject { get; }
        protected PropertyInfo Property { get; }
        protected ISerializerOptions SerializerOptions { get; }

        public virtual string Name
        {
            get
            {
                if (this.Property == null) return null;

                var parentName = this.ParentObject?.Name?.Trim(); // Trims might not be necessarry.
                var propertyName = this.DetermineName(this.Property, this.SerializerOptions?.DefaultNameResolver)?.Trim();

                var returnValue = !string.IsNullOrWhiteSpace(parentName) ? $"{parentName}[{propertyName}]" : propertyName;
                if (this.Property.PropertyType.IsEnumerable()) returnValue += "[]";

                return returnValue;
            }
        }

        protected IContentSerializer GetParent() => this.ParentObject;

        // ReSharper disable once MemberCanBeProtected.Global Justification: Used in Activator.Create.
        public ContentSerializer(
            TValue value,
            IContentSerializer parentObject,
            PropertyInfo property,
            ISerializerOptions serializerOptions) : base(value)
        {
            this.Property = property;
            this.ParentObject = parentObject;
            this.SerializerOptions = serializerOptions;
        }

        protected override HttpContent[] Content()
        {
            //TODO: Turn this into a proper factory, if possible
            if (this.IsEnum)
                return new EnumSerializer<TValue>(this.Value, this.ParentObject, this.Property, this.SerializerOptions).ToContent();

            if (this.IsType<byte[]>())
                return new ByteArraySerializer(this.Value as byte[], this.ParentObject, this.Property, this.SerializerOptions).ToContent();       

            // check to see if object is date and should format is specific way
            if (this.Value is DateTime dateValue)
            {
                var attribute = this.Property.GetCustomAttribute<MultipartAttribute>();
                var format = attribute?.DateTimeFormat ??
                    this.SerializerOptions?.DefaultDateFormat;

                if (string.IsNullOrWhiteSpace(format)) format = "M/d/yyyy HH:mm:ss";
                return new StringSerializer(dateValue.ToString(format), this.ParentObject, this.Property, this.SerializerOptions).ToContent();
            }

            if (this.IsType<string>() || this.PropertyType.IsValueType)
                return new StringSerializer(this.Value.ToString(), this.ParentObject, this.Property, this.SerializerOptions).ToContent();

            if (this.IsType<IEnumerable>())
                return new EnumerableSerializer(this.Value as IEnumerable, this.ParentObject, this.Property, this.SerializerOptions)
                    .ToContent();

            return this.IsClass
                ? new ClassSerializer<TValue>(this.Value, this.ParentObject, this.Property, this.SerializerOptions).ToContent()
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