using System;
namespace TiberHealth.Serializer
{
    public interface ISerializerOptions
    {
        string DefaultDateFormat { get; set; }

        IContractNameResolver DefaultNameResolver { get; set; }
        IContractNameResolver PropertyNameResolver { get; set; }
        IContractNameResolver EnumNameResolver { get; set; }
    }
}
