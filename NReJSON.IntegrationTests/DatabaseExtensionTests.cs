using StackExchange.Redis;
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

        [Fact]
        public void ItCanGetMultipleJsonObjectAtDefaultPath()
        {
            _db.JsonSet("test_multiget_object:1", "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");
            _db.JsonSet("test_multiget_object:2", "{\"hello\": \"tom\", \"goodnight\": {\"value\": \"tom\"}}");

            var result = _db.JsonMultiGet(new RedisKey[] { "test_multiget_object:1", "test_multiget_object:2" });

            Assert.Equal(2, result.Length);
            Assert.Contains("world", result[0].ToString());
            Assert.Contains("tom", result[1].ToString());
        }

        [Fact]
        public void ItCanGetMultipleJsonObjectAtSpecificPath()
        {
            _db.JsonSet("test_multiget_object:1", "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");
            _db.JsonSet("test_multiget_object:2", "{\"hello\": \"tom\", \"goodnight\": {\"value\": \"tom\"}}");

            var result = _db.JsonMultiGet(new RedisKey[] { "test_multiget_object:1", "test_multiget_object:2" }, ".hello");

            Assert.Equal("\"world\"", result[0].ToString());
            Assert.Equal("\"tom\"", result[1].ToString());
        }
    }
}
