using System.Globalization;
using System.Linq;

namespace TiberHealth.Serializer
{
    /// <summary>
    /// Converter routine to change fields in snake_case to PascalCase 
    /// </summary>
    public class SnakeToPascalResolver : IContractNameResolver
    {
        public string ConvertName(string propertyHName)
        {
            var parts =
                propertyHName
                    .Split(' ')
                    .SelectMany(item => item.Split('_').Select(item => item.Trim().ToLower()))
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Select(item => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item))
                    .ToArray();

            var pascalString = string.Join(string.Empty, parts);
            return pascalString;
        }
    }
}
