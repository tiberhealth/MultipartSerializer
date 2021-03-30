using System.Linq;
using System.Net.Http;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NUnit.Framework;
using TiberHealth.Serializer;
using TiberHealth.Serializer.Attributes;

namespace SerializerTest
{
    public interface IClassA
    {
        [JsonProperty("first_name")] string FirstName { get; }
        string LastName { get; }
    }

    public interface IClassB : IClassA
    {
        new string FirstName { get; }   // need to force a duplicate for testing
        [Multipart("favorite_drink")] string FavoriteDrink { get; }
    }
    public class ClassA: IClassA    
    {
        public string FirstName => "Bryan";
        public string LastName => "Lenihan";
    }

    public class ClassB : ClassA, IClassB
    {
        [JsonProperty("fav_drink")] public string FavoriteDrink => "A&W Root Beer Zero Sugar";
    }
    
    internal class InheritHierarchyTests
    {
        [Test]
        public void TestInheritance()
        {
            var request = new ClassB() as IClassB;
            Assert.IsNotNull(request); 
            
            var context = FormDataSerializer.Serialize(request) as MultipartFormDataContent;
            Assert.IsNotNull(context);

            Assert.AreEqual(3, context.Count());

            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"favorite_drink\""));
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"FirstName\""));
            Assert.True(context.Any(field => field.Headers.ContentDisposition.Name == "\"LastName\""));
       
        }

        [Test]
        public void TestConcreteInheritance()
        {
            var concreteRequest = new ClassB();
            var concreteContext = FormDataSerializer.Serialize(concreteRequest) as MultipartFormDataContent;
            Assert.IsNotNull(concreteRequest);

            Assert.AreEqual(3, concreteContext.Count());

            Assert.True(concreteContext.Any(field => field.Headers.ContentDisposition.Name == "\"fav_drink\""));
        }
        
    }
}