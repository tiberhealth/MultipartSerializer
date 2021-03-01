// ReSharper disable All Justification: this is a test class/file

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using NUnit.Framework;
using System.Reflection;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TiberHealth.Serializer.Attributes;
using TiberHealth.Serializer.Extensions;

namespace SerializerTest
{
    public class SerializationTest
    {
        internal class ExpectNotSentAttribute : Attribute
        {
        }

        [Multipart(EnumAsString = true)]
        internal enum TestStringEnum
        {
            testValue1 = 1,
            testValue2 = 2,
            [EnumSerializedValue("peterpan")]testValue3 = 4
        }

        internal enum TestEnum
        {
            value1 = 1,
            value2,
            value3
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

            [Multipart(Name = "LocalState")] public string State => "NC";

            public byte[] File => Encoding.ASCII.GetBytes("this is a test of the emergancy broadcsting system");

            [MultipartFile(Name = "Testfile", FileName = "Fname", ContentType = "MimeT", Value = "JsonBytes")]
            public object TestFile2 => new
            {
                Fname = "TestFile2.json",
                MimeT = "text/json",
                JsonBytes = Encoding.ASCII.GetBytes("{\"name\": \"test\"}")
            };

            public TestEnum EnumTest => TestEnum.value2;
            [Multipart(EnumAsString = true)] public TestEnum EnumAsString => TestEnum.value3;

            [JsonProperty("string-test-enum")] public TestStringEnum StringEnumTest => TestStringEnum.testValue3;

            [Newtonsoft.Json.JsonIgnore, ExpectNotSent]
            public string DoNotSend => "This should not be sent";

            [System.Text.Json.Serialization.JsonIgnore, ExpectNotSent]
            public string DoNotSendTextJson => "This should not be sent";

            [MultipartIgnore, ExpectNotSent] public string DoNotSendMultipartIgnore => "This should not be sent";

            [ExpectNotSent] public DateTime? NullDateTime => null;
            public DateTime ValidDateTime { get; set; }
        }

        [Test]
        public async Task TestMultipartCreation()
        {
            var testDateTime = DateTime.Now;
            var request = new TestObject()
            {
                ValidDateTime = testDateTime
            };
            
            var response = TiberHealth.Serializer.FormDataSerializer.Serialize(request) as MultipartFormDataContent;
            Assert.NotNull(response);

            var expectedProperties = typeof(TestObject)
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<ExpectNotSentAttribute>() == null)
                .ToArray();

            Assert.AreEqual(expectedProperties.Count(), response.Count());
            Assert.IsFalse(response.Any(item => item.Headers.ContentDisposition == null));

            Assert.IsFalse(response.Any(item => item.Headers.ContentDisposition.Name == "\"City\""));
            Assert.IsNotNull(response.SingleOrDefault(item =>
                item.Headers.ContentDisposition.Name == "\"StreetAddress\""));

            Assert.IsFalse(response.Any(item => item.Headers.ContentDisposition.Name == "\"Address\""));
            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"LocalCity\""));

            Assert.IsFalse(response.Any(item => item.Headers.ContentDisposition.Name == "\"State\""));
            Assert.IsNotNull(response.SingleOrDefault(item =>
                item.Headers.ContentDisposition.Name == "\"LocalState\""));

            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"File\""));

            foreach (var property in typeof(TestObject).GetProperties()
                .Where(prop => prop.HasCustomAttribute<ExpectNotSentAttribute>()))
            {
                Assert.IsNull(response.SingleOrDefault(item =>
                    item.Headers.ContentDisposition.Name == $"\"{property.Name}\""));
            }

            Assert.IsAssignableFrom<ByteArrayContent>(response.SingleOrDefault(item =>
                item.Headers.ContentDisposition.Name == "\"File\""));

            var enumValue = await response.Single(item => item.Headers.ContentDisposition.Name == "\"EnumTest\"")
                .ReadAsStringAsync();
            Assert.AreEqual(2, int.Parse(enumValue));

            var enumStringValue = await response.Single(item => item.Headers.ContentDisposition.Name == "\"EnumAsString\"").ReadAsStringAsync();
            Assert.AreEqual("value3", enumStringValue);

            var testEnumString = await response.Single(item => item.Headers.ContentDisposition.Name == "\"string-test-enum\"").ReadAsStringAsync();
            Assert.AreEqual("peterpan", testEnumString);

            var testfile = response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"Testfile\"");
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
                FileBytes = Encoding.ASCII.GetBytes(
                    "this is a test of the emergancy broadcsting system. If this was a real test....")
            };

            public TestFile TestSingleFile2 => new TestFile
            {
                FileName = "Testfile.txt",
                ContentType = null,
                FileBytes = Encoding.ASCII.GetBytes(
                    "this is a test of the emergancy broadcsting system. If this was a real test....")
            };
        }

        [Test]
        public void MultipartFileTest()
        {
            var request = new MultipartTestObject();
            var response = TiberHealth.Serializer.FormDataSerializer.Serialize(request) as MultipartFormDataContent;

            var Testfile =
                response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"SingleFileTest\"");
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
                FileBytes = Encoding.ASCII.GetBytes(
                    "this is a test of the emergancy broadcsting system. If this was a real test....")
            };
        }

        [Test]
        public void NoContentTypeTest()
        {
            var request = new NoContentTestObject();
            var response = TiberHealth.Serializer.FormDataSerializer.Serialize(request) as MultipartFormDataContent;
            Assert.IsNotNull(response);

            var Testfile = response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"TestElement\"");
            Assert.IsNotNull(Testfile);

            Assert.IsInstanceOf<ByteArrayContent>(Testfile);
            Assert.AreEqual("Testfile.txt", Testfile.Headers.ContentDisposition.FileName);
            Assert.IsNull(Testfile.Headers.ContentType);
        }


        private class MultipartIncludeTestObject
        {
            [JsonIgnore, MultipartInclude] public string ShouldBeIncluded => "Bryan";

            [JsonIgnore, Multipart(Name = "AlsoIncluded")]
            public string ShouldAlsoBeIncluded => "Test";
        }

        [Test]
        public void TestMultipartinclude()
        {
            var request = new MultipartIncludeTestObject();
            var response = TiberHealth.Serializer.FormDataSerializer.Serialize(request) as MultipartFormDataContent;
            Assert.IsNotNull(response);

            var testPart =
                response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"ShouldBeIncluded\"");
            Assert.IsNotNull(testPart);

            var testPart2 =
                response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"AlsoIncluded\"");
            Assert.IsNotNull(testPart2);
        }


        [MultipartFile(FileName = "Name", ContentType = "MimeType", Value = "FileBytes")]
        private class AttachmentTestFileObject
        {
            public string Name { get; set; }
            public string MimeType { get; set; }
            public byte[] FileBytes { get; set; }
        }

        private class AttachmentTestObject
        {
            [Multipart(Name = "FilePart")] public IEnumerable<AttachmentTestFileObject> Files { get; set; }

            public string[] StringArray => new[] {"test 1", "test 2", "test 3", "test 4"};

            [Multipart(Name = "Id")] public int[] IntArray => new[] {1, 2, 3, 4, 5};
        }

        [Test]
        public void TestArrayObjects()
        {
            var request = new AttachmentTestObject
            {
                Files = new[]
                {
                    new AttachmentTestFileObject()
                    {
                        Name = "file1.txt", MimeType = "plain/text", FileBytes = Encoding.ASCII.GetBytes("Test file 1")
                    },
                    new AttachmentTestFileObject()
                    {
                        Name = "file2.txt", MimeType = "plain/text", FileBytes = Encoding.ASCII.GetBytes("Test file 2")
                    },
                    new AttachmentTestFileObject()
                    {
                        Name = "file3.txt", MimeType = "plain/text", FileBytes = Encoding.ASCII.GetBytes("Test file 3")
                    }
                }
            };
            var response = TiberHealth.Serializer.FormDataSerializer.Serialize(request) as MultipartFormDataContent;
            Assert.IsNotNull(response);

            var expectedCount = request.Files.Count() + request.StringArray.Count() + request.IntArray.Count();
            Assert.AreEqual(expectedCount, response.Count());

            Assert.AreEqual(request.Files.Count(), response.Where(item =>
                    item.Headers.ContentDisposition.Name == "\"FilePart[]\"" &&
                    new Regex("file\\d.txt").IsMatch(item.Headers.ContentDisposition.FileName)
                )
                .Count()
            );

            Assert.AreEqual(
                request.StringArray.Length,
                response.Where(item => item.Headers.ContentDisposition.Name == "\"StringArray[]\"").Count()
            );

            Assert.AreEqual(
                request.IntArray.Length,
                response.Where(item => item.Headers.ContentDisposition.Name == "\"Id[]\"").Count()
            );
        }

        [Test]
        public void TestLocalAndHierarchy()
        {
            var request = new
            {
                Field1 = "Field1String",
                Field2 = 2,
                Field3 = new
                {
                    Field3Field1 = "Test",
                    Field3Field2 = 2
                },
                field4 = new[] {1, 2, 3}
            };

            var response = TiberHealth.Serializer.FormDataSerializer.Serialize(request) as MultipartFormDataContent;
            Assert.IsNotNull(response);
            
            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"Field1\""));
            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"Field2\""));
            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"Field3[Field3Field1]\""));
            Assert.IsNotNull(response.SingleOrDefault(item => item.Headers.ContentDisposition.Name == "\"Field3[Field3Field2]\""));
            
            Assert.AreEqual(0, response.Count(item => item.Headers.ContentDisposition.Name == "\"Field4[]\""));
            Assert.AreEqual(request.field4.Length, response.Count(item => item.Headers.ContentDisposition.Name == "\"field4[]\""));
        }

        public class NullableTestObject
        {
            public DateTime? NullDate => null;
            public DateTime? PopulatedNullableDate { get; set; }
        }

        [Test]
        public async Task TestNullables()
        {
            var testDateTime = DateTime.Now.ToUniversalTime();
            var request = new NullableTestObject()
            {
                PopulatedNullableDate = testDateTime
            };
            var response = TiberHealth.Serializer.FormDataSerializer.Serialize(request) as MultipartFormDataContent;
            Assert.IsNotNull(response);
           
            Assert.IsNull(response.FirstOrDefault(item => item.Headers.ContentDisposition.Name == "\"NullDate\""));

            var content =
                response.FirstOrDefault(item => item.Headers.ContentDisposition.Name == "\"PopulatedNullableDate\"");
            Assert.IsNotNull(content);
            var results = await content.ReadAsStringAsync();
            Assert.IsNotNull(results);
        }
    }
}