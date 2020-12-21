using System.Net.Http;
using System.Linq;
using TiberHealth.Serializer.ContentSerializers;

namespace TiberHealth.Serializer
{
    public class FormDataSerializer
    {
        /// <summary>
        /// Serialize an object to a Multipart Form Data Content
        /// <seealso cref="System.Net.Http.MultipartFormDataContent"/>
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="bodyObject"></param>
        /// <returns></returns>
        public static HttpContent FormDataContent<TBody>(TBody bodyObject) where TBody : class => new FormDataSerializer<TBody>(bodyObject).ToContent();
     }

    public class FormDataSerializer<TBody> : SerializerBase
        where TBody: class
    {
        private MultipartFormDataContent Form;

        internal FormDataSerializer(object bodyObject): base(bodyObject)
        {
            this.Form = new MultipartFormDataContent(); 
        }

        protected override HttpContent Content()
        {
            if (this.BodyObject == null) return this.Form;

            typeof(TBody).GetProperties()
                            .Where(prop => prop.IsNotIgnore())
                            .Select(prop => new ContentSerializer(prop, this.BodyObject).ToContent())
                            .Where(item => item != null)
                            .ToList()
                            .ForEach(part => this.Form.Add(part));

            return this.Form;
        }
    }
}
