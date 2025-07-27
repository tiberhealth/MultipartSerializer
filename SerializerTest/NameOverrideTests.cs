namespace SerializerTest
{
    public class NameOverrideTests
    {
        [EnumAsString]
        internal enum TestEnum
        {
            ValueOne,
            ValueTwo
        }

        internal class TestClass
        {
            public int FieldA => 7;
            public string FieldB => "Test field B";
            [Multipart("FieldC")] public int DefaultFieldC => 76;
            [Multipart("Field_D")] public int DefaultFieldD => 72;
            public TestEnum EnumField => TestEnum.ValueTwo;
        }

        [Test]
        public async Task TestNameOverrideTest()
        {
            var testObject = new TestClass();

            var defaultContext = FormDataSerializer.Serialize(testObject) as MultipartFormDataContent;
            Assert.True(defaultContext.Any(field => field.Headers.ContentDisposition.Name == "\"FieldA\""));
            Assert.True(defaultContext.Any(field => field.Headers.ContentDisposition.Name == "\"FieldB\""));
            Assert.True(defaultContext.Any(field => field.Headers.ContentDisposition.Name == "\"FieldC\""));
            Assert.True(defaultContext.Any(field => field.Headers.ContentDisposition.Name == "\"Field_D\""));
            var defaultEnumValue = await defaultContext.Single(item => item.Headers.ContentDisposition.Name == "\"EnumField\"").ReadAsStringAsync();
            Assert.IsNotNull(defaultEnumValue);
            Assert.AreEqual("ValueTwo", defaultEnumValue);


            var context = FormDataSerializer.Serialize(testObject, options =>
            {
                options.DefaultNameResolver = new PascalToSnakeResolver();
            }) as MultipartFormDataContent;

            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"field_a\""));
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"field_b\""));
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"FieldC\""));
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"Field_D\""));
            var contextEnumValue = await context.Single(item => item.Headers.ContentDisposition.Name == "\"enum_field\"").ReadAsStringAsync();
            Assert.IsNotNull(contextEnumValue);
            Assert.AreEqual("ValueTwo", contextEnumValue);

            var enumContext = FormDataSerializer.Serialize(testObject, options => options.EnumNameResolver = new PascalToSnakeResolver()) as MultipartFormDataContent;
            Assert.True(defaultContext.Any(field => field.Headers.ContentDisposition.Name == "\"FieldA\""));
            Assert.True(defaultContext.Any(field => field.Headers.ContentDisposition.Name == "\"FieldB\""));
            Assert.True(defaultContext.Any(field => field.Headers.ContentDisposition.Name == "\"FieldC\""));
            Assert.True(defaultContext.Any(field => field.Headers.ContentDisposition.Name == "\"Field_D\""));

            var enumValue = await enumContext.Single(item => item.Headers.ContentDisposition.Name == "\"EnumField\"").ReadAsStringAsync();
            Assert.IsNotNull(enumValue);
            Assert.AreEqual("value_two", enumValue);

            var enumContext2 = FormDataSerializer.Serialize(testObject, options =>
            {
                options.EnumNameResolver = new PascalToSnakeResolver();
                options.DefaultNameResolver = options.EnumNameResolver;
            }) as MultipartFormDataContent;
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"field_a\""));
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"field_b\""));
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"FieldC\""));
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"Field_D\""));

            var enumValue2 = await enumContext2.Single(item => item.Headers.ContentDisposition.Name == "\"enum_field\"").ReadAsStringAsync();
            Assert.IsNotNull(enumValue2);
            Assert.AreEqual("value_two", enumValue2);

        }

    }
}
