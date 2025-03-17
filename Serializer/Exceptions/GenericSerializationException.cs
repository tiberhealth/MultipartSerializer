namespace TiberHealth.Serializer.Exceptions
{
    public class GenericSerializationException: Exception
    {
        public GenericSerializationException(string message, Exception ex = null): base(message, ex)
        {
        }
    }
}
