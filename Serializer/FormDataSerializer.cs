using System.Net.Http;
using System.Linq;
using TiberHealth.Serializer.ContentSerializers;
using System;

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
        public static HttpContent Serialize<TBody>(TBody bodyObject, Action<ISerializerOptions> optionsFactory = null, string boundry = null) where TBody : class =>
            new FormDataSerializer<TBody>(bodyObject).Serialize(optionsFactory, boundry);
     }

    public class FormDataSerializer<TBody> : SerializerBase<TBody>
        where TBody: class
    {
        private ISerializerOptions SerializerOptions { get; set;  }
        internal FormDataSerializer(TBody bodyObject): base(bodyObject) { }

        protected override HttpContent[] Content() =>
            new ClassSerializer<TBody>(this.Value, this.SerializerOptions).ToContent();

        public HttpContent Serialize(Action<ISerializerOptions> serializerOptionsFactory = null, string boundary = null)
        {
            var form = string.IsNullOrWhiteSpace(boundary) ? new MultipartFormDataContent() : new MultipartFormDataContent(boundary);

            if (serializerOptionsFactory != null)
            {
                this.SerializerOptions = new SerializerOptions();
                serializerOptionsFactory?.Invoke(this.SerializerOptions);
            }

            this.ToContent()
                .ToList()
                .ForEach(item => form.Add(item));

            return form;
        }
    }
}
