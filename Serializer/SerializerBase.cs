using System.Net.Http;

namespace TiberHealth.Serializer
{
    public abstract class SerializerBase
    {
        protected object BodyObject { get; }
        protected abstract HttpContent Content();

        public SerializerBase(object bodyObject) => this.BodyObject = bodyObject;

        public virtual HttpContent ToContent() => this.Content();
    }
}
