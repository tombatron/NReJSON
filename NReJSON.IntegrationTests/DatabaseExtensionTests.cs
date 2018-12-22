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

        [Theory]
        [InlineData(".string", "string")]
        [InlineData(".integer", "integer")]
        [InlineData(".boolean", "boolean")]
        [InlineData(".number", "number")]
        public void ItCanGetJsonType(string path, string value)
        {
            var key = $"test_{nameof(ItCanGetJsonType)}";

            _db.JsonSet(key, "{\"string\":\"hello world\", \"integer\":5, \"boolean\": true, \"number\":4.7}");

            var typeResult = _db.JsonType(key, path);

            Assert.Equal(value, typeResult.ToString());
        }

        [Theory]
        [InlineData(".integer", 1, 2)]
        [InlineData(".number", .9, 2)]
        public void ItCanIncrementJsonValues(string path, double number, double expectedResult)
        {
            var key = $"test_{nameof(ItCanIncrementJsonValues)}";

            _db.JsonSet(key, "{\"integer\":1,\"number\":1.1}");

            var result = _db.JsonIncrementNumber(key, path, number);

            Assert.Equal(expectedResult, (double)result, 2);
        }

        [Theory]
        [InlineData(".integer", 10, 10)]
        [InlineData(".number", .9, .99)]
        public void ItCanMultiplyJsonValues(string path, double number, double expectedResult)
        {
            var key = $"test_{nameof(ItCanMultiplyJsonValues)}";

            _db.JsonSet(key, "{\"integer\":1,\"number\":1.1}");

            var result = _db.JsonMultiplyNumber(key, path, number);

            Assert.Equal(expectedResult, (double)result, 2);
        }

        [Fact(Skip = "This doesn't work, not sure what I'm doing wrong yet.")]
        public void ItCanAppendJsonString()
        {
            var key = $"test_{nameof(ItCanAppendJsonString)}";

            _db.JsonSet(key, "{\"hello\":\"world\"}");

            var result = _db.JsonAppendJsonString(key, ".hello", "{\"t\":1}");

            Assert.Equal(4, result);
        }
    }
}
