using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using NUnit.Framework;
using System.Reflection;
using TiberHealth.Serializer.Extensions;
using TiberHealth.Serializer;
using System.Net.Http.Headers;

namespace SerializerTest
{
    public class SerializationTest
    {
        internal class ExpectNotSentAttribute : Attribute { }
        internal enum TestEnum
        {
            value1 = 1,
            value2
        }

        [MultipartFile(FileName = "FileName", ContentType = "ContentType", Value = "FileBytes", Name = "Attachment")]
        internal class TestFile
        {
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public byte[] FileBytes { get; set; }
        }

        internal class TestObject
        {
            public string Name => "Test Name";

            [Newtonsoft.Json.JsonProperty("StreetAddress")]
            public string Address => "951 Main Street";

            [System.Text.Json.Serialization.JsonPropertyName("LocalCity")]
            public string City => "Rolesville";

            [TiberHealth.Serializer.Multipart(Name = "LocalState")]
            public string State => "NC";

            public byte[] File => Encoding.ASCII.GetBytes("this is a test of the emergancy broadcsting system");


            [MultipartFile(Name = "Testfile2", FileName = "Fname", ContentType = "MimeT", Value = "JsonBytes")]
            public object TestFile2 => new 
            {
                Fname = "TestFile2.json",
                MimeT = "text/json",
                JsonBytes = Encoding.ASCII.GetBytes("{\"name\": \"test\"}")
            };

            public TestEnum EnumTest => TestEnum.value2;

            [Newtonsoft.Json.JsonIgnore, ExpectNotSent]
            public string DoNotSend => "This should not be sent";

            [System.Text.Json.Serialization.JsonIgnore, ExpectNotSent]
            public string DoNotSendTextJson => "This should not be sent";

            [TiberHealth.Serializer.MultipartIgnore, ExpectNotSent]
            public string DoNotSendMultipartIgnore => "This should not be sent";

        }

        [Test]
        public async Task TestMultipartCreation()
        {
            var request = new TestObject();
            var response = TiberHealth.Serializer.FormDataSerializer.FormDataContent(request) as MultipartFormDataContent;

            Assert.NotNull(response);

            var expectedProperties = typeof(TestObject)
                                    .GetProperties()
                                    .Where(prop => prop.GetCustomAttribute<ExpectNotSentAttribute>() == null)
                                    .ToArray();

            Assert.AreEqual(expectedProperties.Count(), response.Count());
            Assert.IsFalse(response.Any(item => item.Headers.ContentDisposition == null));

            Assert.IsFalse(response.Any(item => item.Headers.ContentDisposition.Name == "\"City\""));
            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"StreetAddress\""));

            Assert.IsFalse(response.Any(item => item.Headers.ContentDisposition.Name == "\"Address\""));
            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"LocalCity\""));

            Assert.IsFalse(response.Any(item => item.Headers.ContentDisposition.Name == "\"State\""));
            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"LocalState\""));

            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"File\""));

            foreach (var property in typeof(TestObject).GetProperties().Where(prop => prop.HasCustomAttribute<ExpectNotSentAttribute>()))
            {
                Assert.IsNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == $"\"{property.Name}\""));
            }

            Assert.IsAssignableFrom<ByteArrayContent>(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"File\""));

            var enumValue = await response.Single(item => item.Headers.ContentDisposition.Name == "\"EnumTest\"").ReadAsStringAsync();
            Assert.AreEqual(2, int.Parse(enumValue));

            var testfile = response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"Testfile2\"");
            Assert.IsNotNull(testfile);
            Assert.AreEqual("TestFile2.json", testfile.Headers.ContentDisposition.FileName);
            Assert.AreEqual(MediaTypeHeaderValue.Parse("text/json"), testfile.Headers.ContentType);
        }

        private class MultipartTestObject
        {
            [MultipartFile(Name = "SingleFileTest", Value = "FileBytes")]
            public TestFile TestSingleFile => new TestFile
            {
                FileName = "Testfile.txt",
                ContentType = "text/plain",
                FileBytes = Encoding.ASCII.GetBytes("this is a test of the emergancy broadcsting system. If this was a real test....")
            };
        }

        [Test]
        public void MultipartFileTest()
        {
            var request = new MultipartTestObject();
            var response = TiberHealth.Serializer.FormDataSerializer.FormDataContent(request) as MultipartFormDataContent;

            var Testfile = response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"SingleFileTest\"");
            Assert.IsNotNull(Testfile);

            Assert.IsInstanceOf<ByteArrayContent>(Testfile);
            Assert.AreEqual("Testfile.txt", Testfile.Headers.ContentDisposition.FileName);
            Assert.AreEqual(MediaTypeHeaderValue.Parse("text/plain"), Testfile.Headers.ContentType);
        }

        private class NoContentTestObject
        {
            [MultipartFile]
            public TestFile TestElement => new TestFile
            {
                FileName = "Testfile.txt",
                FileBytes = Encoding.ASCII.GetBytes("this is a test of the emergancy broadcsting system. If this was a real test....")
            };
        }

        [Test]
        public void NoContentTypeTest()
        {
            var request = new NoContentTestObject();
            var response = TiberHealth.Serializer.FormDataSerializer.FormDataContent(request) as MultipartFormDataContent;
            Assert.IsNotNull(response);

            var Testfile = response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"TestElement\"");
            Assert.IsNotNull(Testfile);

            Assert.IsInstanceOf<ByteArrayContent>(Testfile);
            Assert.AreEqual("Testfile.txt", Testfile.Headers.ContentDisposition.FileName);
            Assert.IsNull(Testfile.Headers.ContentType);
        }
    }
}
