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
            var defaultFormat = "M/d/yyyy HH:mm:ss";
            var testFormat = "yyyy-MM-dd HH:mm:ss";

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
            Assert.AreEqual(formattedDateTime.ToString(testFormat), formattedDateTest);

            var normalDateTest = await response.Single(item => item.Headers.ContentDisposition.Name == "\"normal-date\"").ReadAsStringAsync();
            Assert.AreNotEqual(normalDateTime.ToString(testFormat), normalDateTest);
            Assert.AreEqual(normalDateTime.ToString(defaultFormat), normalDateTest);

            var globalFormat = "M/d/yyyy HH:mm:sszzzz";
            var globalDateFormatResponse = TiberHealth.Serializer.FormDataSerializer.Serialize(request, options =>
            {
                options.DefaultDateFormat = globalFormat;
            }) as MultipartFormDataContent;

            Assert.IsNotNull(globalDateFormatResponse);

            var formattedDateTest2 = await globalDateFormatResponse.Single(item => item.Headers.ContentDisposition.Name == "\"formatted-date\"").ReadAsStringAsync();
            Assert.AreEqual(formattedDateTime.ToString(testFormat), formattedDateTest2);

            var normalDateTest2 = await globalDateFormatResponse.Single(item => item.Headers.ContentDisposition.Name == "\"normal-date\"").ReadAsStringAsync();
            Assert.AreNotEqual(normalDateTime.ToString(testFormat), normalDateTest2);
            Assert.AreNotEqual(normalDateTime.ToString(defaultFormat), normalDateTest2);
            Assert.AreEqual(normalDateTime.ToString(globalFormat), normalDateTest2);

        }
    }
}
