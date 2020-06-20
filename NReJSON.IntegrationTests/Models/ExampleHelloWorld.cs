using System.Text.Json.Serialization;

namespace NReJSON.IntegrationTests.Models
{
    public class ExampleHelloWorld
    {
        public class InnerExample
        {
            [JsonPropertyName("value")]
            public string Value { get; set; }
        }

        [JsonPropertyName("hello")]
        public string Hello { get; set; }

        [JsonPropertyName("goodnight")]
        public InnerExample GoodNight { get; set; }
    }
}