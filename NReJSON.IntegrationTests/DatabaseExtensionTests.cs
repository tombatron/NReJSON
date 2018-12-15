using Xunit;

namespace NReJSON.IntegrationTests
{
    public class DatabaseExtensionTests : BaseIntegrationTest
    {
        [Fact]
        public void ItCanSetJson()
        {
            var result = _db.JsonSet("test_key_set", "{}");

            Assert.NotNull(result);
        }

        [Fact]
        public void ItCanGetAJsonObject()
        {
            _db.JsonSet("test_get_object", "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

            var result = _db.JsonGet("test_get_object");

            Assert.False(result.IsNull);
        }

        [Fact]
        public void ItCanGetJsonAtASinglePath()
        {
            _db.JsonSet("test_get_object", "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

            var result = _db.JsonGet("test_get_object", ".hello");

            Assert.Equal("\"world\"", result.ToString());
        }

        [Fact]
        public void ItCanGetJsonAtMultiplePaths()
        {
            _db.JsonSet("test_get_object", "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

            var result = _db.JsonGet("test_get_object", ".hello", ".goodnight.value");

            Assert.Equal("{\".hello\":\"world\",\".goodnight.value\":\"moon\"}", result.ToString());
        }
    }
}
