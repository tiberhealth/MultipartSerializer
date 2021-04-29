using System;
namespace TiberHealth.Serializer
{
    public interface IContractNameResolver
    {
        string ConvertName(string propertyHName); 
    }
}
