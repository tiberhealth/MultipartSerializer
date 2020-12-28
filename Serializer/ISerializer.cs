using System.Net.Http;

namespace TiberHealth.Serializer
{
    public interface ISerializer
    {
        HttpContent[] ToContent();
    }
}
