using System;
namespace TiberHealth.Serializer
{
    public class DefaultResolver : IContractNameResolver
    {
        public string ConvertName(string propertyHName) => propertyHName;
    }
}
