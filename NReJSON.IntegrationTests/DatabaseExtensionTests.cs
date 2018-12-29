using System;
using System.Linq;
using StackExchange.Redis;
using Xunit;

namespace NReJSON.IntegrationTests
{
    public class DatabaseExtensionTests
    {
        public class JsonSet : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString("N");

                var result = _db.JsonSet(key, "{}");

                Assert.NotNull(result);
            }
        }

        public class JsonGet : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGet("test_get_object");

                Assert.False(result.IsNull);
            }
        }

        public class JsonDelete : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonDelete(key, ".goodnight");
                var jsonResult = _db.JsonGet(key);

                Assert.Equal(1, result);
                Assert.DoesNotContain("goodnight", jsonResult.ToString());
            }
        }

        public class JsonMultiGet : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key1 = Guid.NewGuid().ToString("N");
                var key2 = Guid.NewGuid().ToString("N");

                _db.JsonSet(key1, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");
                _db.JsonSet(key2, "{\"hello\": \"tom\", \"goodnight\": {\"value\": \"tom\"}}");

                var result = _db.JsonMultiGet(new RedisKey[] { key1, key2 });

                Assert.Equal(2, result.Length);
                Assert.Contains("world", result[0].ToString());
                Assert.Contains("tom", result[1].ToString());
            }
        }

        public class JsonType : BaseIntegrationTest
        {
            [Theory]
            [InlineData(".string", "string")]
            [InlineData(".integer", "integer")]
            [InlineData(".boolean", "boolean")]
            [InlineData(".number", "number")]
            public void CanExecute(string path, string value)
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"string\":\"hello world\", \"integer\":5, \"boolean\": true, \"number\":4.7}");

                var typeResult = _db.JsonType(key, path);

                Assert.Equal(value, typeResult.ToString());
            }
        }

        public class JsonIncrementNumber : BaseIntegrationTest
        {
            [Theory]
            [InlineData(".integer", 1, 2)]
            [InlineData(".number", .9, 2)]
            public void CanExecute(string path, double number, double expectedResult)
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"integer\":1,\"number\":1.1}");

                var result = _db.JsonIncrementNumber(key, path, number);

                Assert.Equal(expectedResult, (double)result, 2);
            }
        }

        public class JsonMultiplyNumber : BaseIntegrationTest
        {
            [Theory]
            [InlineData(".integer", 10, 10)]
            [InlineData(".number", .9, .99)]
            public void CanExecute(string path, double number, double expectedResult)
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"integer\":1,\"number\":1.1}");

                var result = _db.JsonMultiplyNumber(key, path, number);

                Assert.Equal(expectedResult, (double)result, 2);
            }
        }

        public class JsonAppendJsonString : BaseIntegrationTest
        {
            [Fact(Skip = "This doesn't work, not sure what I'm doing wrong yet.")]
            public void ItCanAppendJsonString()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\":\"world\"}");

                var result = _db.JsonAppendJsonString(key, ".hello", "{\"t\":1}");

                Assert.Equal(4, result);
            }
        }

        public class JsonStringLength : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\":\"world\"}");

                var result = _db.JsonStringLength(key, ".hello");

                Assert.Equal(5, result);
            }
        }

        public class JsonArrayAppend : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"array\": []}");

                var result = _db.JsonArrayAppend(key, ".array", "\"hello\"", "\"world\"");

                Assert.Equal(2, result);
            }
        }

        public class JsonArrayIndexOf : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = _db.JsonArrayIndexOf(key, ".array", "\"world\"", 0, 2);

                Assert.Equal(1, result);
            }
        }

        public class JsonArrayInsert : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = _db.JsonArrayInsert(key, ".array", 1, "\"there\"");

                Assert.Equal(4, result);
            }
        }

        public class JsonArrayLength : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = _db.JsonArrayLength(key, ".array");

                Assert.Equal(3, result);
            }
        }

        public class JsonArrayPop : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = _db.JsonArrayPop(key, ".array", 1);

                Assert.Equal("\"world\"", result.ToString());
            }
        }

        public class JsonArrayTrim : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = _db.JsonArrayTrim(key, ".array", 0, 1);

                Assert.Equal(2, result);
            }
        }

        public class JsonObjectKeys : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonObjectKeys(key);

                Assert.Equal(new[] { "hello", "goodnight" }, result.Select(x => x.ToString()).ToArray());
            }
        }

        public class JsonObjectLength : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonObjectLength(key, ".goodnight");

                Assert.Equal(1, result);
            }
        }

        public class JsonDebugMemory : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonDebugMemory(key, ".goodnight");

                Assert.Equal(89, result);
            }
        }

        public class JsonGetResp : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = ((RedisResult[])_db.JsonGetResp(key)[1])[1];

                Assert.Equal("world", result.ToString());
            }
        }
    }
}