using System.Text.RegularExpressions;

namespace TiberHealth.Serializer
{
    /// <summary>
    /// Converts properties that are in PascalCase to snake_case
    /// </summary>
    public class PascalToSnakeResolver : IContractNameResolver
    {
        public string ConvertName(string propertyHName)
        {
            var convertRegex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) | 
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            var underScoreString = convertRegex.Replace(propertyHName, "_").ToLower();

            return underScoreString;
        }
    }
}
