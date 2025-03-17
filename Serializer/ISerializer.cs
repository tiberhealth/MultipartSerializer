namespace TiberHealth.Serializer
{
    public interface ISerializer
    {
        HttpContent[] ToContent();
    }
}
