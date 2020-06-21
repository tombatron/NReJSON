using System.Text.Json.Serialization;

namespace NReJSON.IntegrationTests.Models
{
    public class ExamplePerson
    {
        [JsonPropertyName("first")]
        public string FirstName { get; set; }

        [JsonPropertyName("last")]
        public string LastName { get; set; }
    }
}