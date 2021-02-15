using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TiberHealth.Serializer;

namespace SerializerTest
{
    public interface IClassA
    {
        string FirstName { get; }
        string LastName { get; }
    }

    public interface IClassB : IClassA
    {
        string FirstName { get; }   // need to force a duplicate for testing
        string FavoriteDrink { get; }
    }
    public class ClassA: IClassA    
    {
        public string FirstName => "Bryan";
        public string LastName => "Lenihan";
    }

    public class ClassB : ClassA, IClassB
    {
        public string FavoriteDrink => "A&W Root Beer Zero Sugar";
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
        }
        
    }
}