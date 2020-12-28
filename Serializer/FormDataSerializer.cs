using System.Net.Http;
using System.Linq;
using TiberHealth.Serializer.ContentSerializers;

namespace TiberHealth.Serializer
{
    public static class FormDataSerializer
    {
        /// <summary>
        /// Serialize an object to a Multipart Form Data Content
        /// <seealso cref="System.Net.Http.MultipartFormDataContent"/>
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="bodyObject"></param>
        /// <returns></returns>
        public static HttpContent Serialize<TBody>(TBody bodyObject) where TBody : class =>
            new FormDataSerializer<TBody>(bodyObject).Serialize();
     }

    public class FormDataSerializer<TBody> : SerializerBase<TBody>
        where TBody: class
    {
        internal FormDataSerializer(TBody bodyObject): base(bodyObject) { }

        protected override HttpContent[] Content() =>
            new ClassSerializer<TBody>(this.Value).ToContent();

        public HttpContent Serialize(string boundary = null)
        {
            var form = string.IsNullOrWhiteSpace(boundary) ? new MultipartFormDataContent() : new MultipartFormDataContent(boundary);

            this.ToContent()
                .ToList()
                .ForEach(item => form.Add(item));

            return form;
        }
    }
}
