namespace TiberHealth.Serializer.Exceptions
{
    public class InvalidTypeException : GenericSerializationException
    {
        public InvalidTypeException(string message, Exception innerException) : base(message, innerException)
        { }

        public InvalidTypeException(string message) : base(message)
        { }
    }
}
