using System;
namespace TiberHealth.Serializer
{
    public interface ISerializerOptions
    {
        IContractNameResolver DefaultNameResolver { get; set; }
        IContractNameResolver PropertyNameResolver { get; set; }
        IContractNameResolver EnumNameResolver { get; set; }
    }
}
