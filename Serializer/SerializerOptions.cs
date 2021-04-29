using System;
namespace TiberHealth.Serializer
{
    internal class SerializerOptions : ISerializerOptions
    {
        public IContractNameResolver DefaultNameResolver { get; set; }
        public IContractNameResolver PropertyNameResolver { get; set; }
        public IContractNameResolver EnumNameResolver { get; set; }
    }
}

