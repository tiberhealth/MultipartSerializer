using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using TiberHealth.Serializer.Attributes;

namespace SerializerTest
{
    public class DateSerializerTest
    {
        internal class TestObject
        {
            [Multipart("formatted-date", DateTimeFormat = "yyyy-MM-dd HH:mm:ss")] public DateTime FormattedDateTime { get; set; }
            [JsonProperty("normal-date")] public DateTime NormalDateTime { get; set; }
        }

        [Test]
        public async Task TestDateSerialization()
        {
            var formattedDateTime = DateTime.Now.AddDays(-10);
            var normalDateTime = DateTime.Now.AddDays(+10);

            var request = new TestObject()
            {
                FormattedDateTime = formattedDateTime,
                NormalDateTime = normalDateTime
            };

            var response = TiberHealth.Serializer.FormDataSerializer.Serialize(request) as MultipartFormDataContent;
            Assert.NotNull(response);

            var formattedDateTest = await response.Single(item => item.Headers.ContentDisposition.Name == "\"formatted-date\"").ReadAsStringAsync();
            Assert.AreEqual(formattedDateTime.ToString("yyyy-MM-dd HH:mm:ss"), formattedDateTest);

            var normalDateTest = await response.Single(item => item.Headers.ContentDisposition.Name == "\"normal-date\"").ReadAsStringAsync();
            Assert.AreNotEqual(normalDateTime.ToString("yyyy-MM-dd HH:mm:ss"), normalDateTest);
            Assert.AreEqual(normalDateTime.ToString("MM/dd/yyyy HH:mm:ss"), normalDateTest);
        }


    }
}
