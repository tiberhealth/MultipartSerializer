namespace TiberHealth.Serializer
{
    internal class SerializerOptions : ISerializerOptions
    {
        public string DefaultDateFormat { get; set; }
        public IContractNameResolver DefaultNameResolver { get; set; }
        public IContractNameResolver PropertyNameResolver { get; set; }
        public IContractNameResolver EnumNameResolver { get; set; }
    }
}

