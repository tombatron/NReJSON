using System;
using System.Linq;
using System.Threading.Tasks;
using NReJSON.IntegrationTests.Models;
using StackExchange.Redis;
using Xunit;

namespace NReJSON.IntegrationTests
{
    public class DatabaseExtensionAsyncTests
    {
        public class JsonSetAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString("N");

                var result = await _db.JsonSetAsync(key, "{}");

                Assert.True(result);
            }

            [Fact]
            public async Task CanExecuteWithSerializer()
            {
                var key = Guid.NewGuid().ToString("N");

                var obj = new ExampleHelloWorld
                {
                    Hello = "World",

                    GoodNight = new ExampleHelloWorld.InnerExample
                    {
                        Value = "Moon"
                    }
                };

                var result = await _db.JsonSetAsync(key, obj);

                Assert.True(result);
            }
        }

        public class JsonGetAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonGetAsync(key);

                Assert.False(result.IsNull);
            }

            [Fact]
            public async Task CanExecuteAsyncWithIdent()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonGetAsync(key, indent: "&");

                Assert.Contains("&", (string)result);
            }

            [Fact]
            public async Task CanExecuteAsyncWithNewline()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonGetAsync(key, newline: "=");

                Assert.Contains("=", (string)result);
            }

            [Fact]
            public async Task CanExecuteAsyncWithSpace()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonGetAsync(key, space: "+");

                Assert.Contains("+", (string)result);
            }

            [Fact]
            public async Task CanExecuteWithSerializer()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonGetAsync<ExampleHelloWorld>(key);

                Assert.NotNull(result);
                Assert.Equal("world", result.Hello);
                Assert.Equal("moon", result.GoodNight.Value);
            }
        }

        public class JsonDeleteAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonDeleteAsync(key, ".goodnight");
                var jsonResult = await _db.JsonGetAsync(key);

                Assert.Equal(1, result);
                Assert.DoesNotContain("goodnight", jsonResult.ToString());
            }
        }

        public class JsonMultiGetAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key1 = Guid.NewGuid().ToString("N");
                var key2 = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key1, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");
                await _db.JsonSetAsync(key2, "{\"hello\": \"tom\", \"goodnight\": {\"value\": \"tom\"}}");

                var result = await _db.JsonMultiGetAsync(new RedisKey[] { key1, key2 });

                Assert.Equal(2, result.Length);
                Assert.Contains("world", result[0].ToString());
                Assert.Contains("tom", result[1].ToString());
            }

            [Fact]
            public async Task CanExecuteWithSerializer()
            {
                var key1 = Guid.NewGuid().ToString("N");
                var key2 = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key1, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");
                await _db.JsonSetAsync(key2, "{\"hello\": \"tom\", \"goodnight\": {\"value\": \"tom\"}}");

                var result = (await _db.JsonMultiGetAsync<ExampleHelloWorld>(new RedisKey[] { key1, "say what?", key2 })).ToList();

                Assert.Equal(3, result.Count);
                Assert.Null(result[1]);
                Assert.Contains("world", result[0].Hello);
                Assert.Contains("tom", result[2].Hello);
            }
        }

        public class JsonTypeAsync : BaseIntegrationTest
        {
            [Theory]
            [InlineData(".string", "string")]
            [InlineData(".integer", "integer")]
            [InlineData(".boolean", "boolean")]
            [InlineData(".number", "number")]
            public async Task CanExecuteAsync(string path, string value)
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"string\":\"hello world\", \"integer\":5, \"boolean\": true, \"number\":4.7}");

                var typeResult = await _db.JsonTypeAsync(key, path);

                Assert.Equal(value, typeResult.ToString());
            }
        }

        public class JsonIncrementNumberAsync : BaseIntegrationTest
        {
            [Theory]
            [InlineData(".integer", 1, 2)]
            [InlineData(".number", .9, 2)]
            public async Task CanExecuteAsync(string path, double number, double expectedResult)
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"integer\":1,\"number\":1.1}");

                var result = await _db.JsonIncrementNumberAsync(key, path, number);

                Assert.Equal(expectedResult, (double)result, 2);
            }
        }

        public class JsonMultiplyNumberAsync : BaseIntegrationTest
        {
            [Theory]
            [InlineData(".integer", 10, 10)]
            [InlineData(".number", .9, .99)]
            public async Task CanExecuteAsync(string path, double number, double expectedResult)
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"integer\":1,\"number\":1.1}");

                var result = await _db.JsonMultiplyNumberAsync(key, path, number);

                Assert.Equal(expectedResult, (double)result, 2);
            }
        }

        public class JsonAppendJsonStringAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"hello\":\"world\"}");

                var result = await _db.JsonAppendJsonStringAsync(key, ".hello", "\"!\"");

                Assert.Equal(6, result);
            }

            [Fact]
            public async Task WillAppendProvidedJsonStringIntoExistingJsonString()
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"hello\":\"world\"}");

                await _db.JsonAppendJsonStringAsync(key, ".hello", "\"!\"");

                var helloValue = await _db.JsonGetAsync<string>(key, ".hello");

                Assert.Equal("world!", helloValue);
            }

            [Fact]
            public async Task WillApendProvidedJsonStringIntoRootIfNoPathProvided()
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "\"world\"");

                await _db.JsonAppendJsonStringAsync(key, jsonString: "\"!\"");

                var helloValue = await _db.JsonGetAsync<string>(key);

                Assert.Equal("world!", helloValue);
            }
        }

        public class JsonStringLengthAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"hello\":\"world\"}");

                var result = await _db.JsonStringLengthAsync(key, ".hello");

                Assert.Equal(5, result);
            }

            [Fact]
            public async Task WillReturnNullIfPathDoesntExist()
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"hello\":\"world\"}");

                var result = await _db.JsonStringLengthAsync("doesnt_exist", ".hello.doesnt.exist");

                Assert.Null(result);
            }
        }

        public class JsonArrayAppendAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString("N");

                await _db.JsonSetAsync(key, "{\"array\": []}");

                var result = await _db.JsonArrayAppendAsync(key, ".array", "\"hello\"", "\"world\"");

                Assert.Equal(2, result);
            }
        }

        public class JsonArrayIndexOfAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = await _db.JsonArrayIndexOfAsync(key, ".array", "\"world\"", 0, 2);

                Assert.Equal(1, result);
            }
        }

        public class JsonArrayInsertAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = await _db.JsonArrayInsertAsync(key, ".array", 1, "\"there\"");

                Assert.Equal(4, result);
            }
        }

        public class JsonArrayLengthAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = await _db.JsonArrayLengthAsync(key, ".array");

                Assert.Equal(3, result);
            }
        }

        public class JsonArrayPopAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = await _db.JsonArrayPopAsync(key, ".array", 1);

                Assert.Equal("\"world\"", result.ToString());
            }

            [Fact]
            public async Task CanExecuteWithSerializerAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = await _db.JsonArrayPopAsync<string>(key, ".array", 1);

                Assert.Equal("world", result);
            }
        }

        public class JsonArrayTrimAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = await _db.JsonArrayTrimAsync(key, ".array", 0, 1);

                Assert.Equal(2, result);
            }
        }

        public class JsonObjectKeysAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonObjectKeysAsync(key);

                Assert.Equal(new[] { "hello", "goodnight" }, result.Select(x => x.ToString()).ToArray());
            }
        }

        public class JsonObjectLengthAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonObjectLengthAsync(key, ".goodnight");

                Assert.Equal(1, result);
            }
        }

        public class JsonDebugMemoryAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = await _db.JsonDebugMemoryAsync(key, ".goodnight");

                Assert.True(result > 50);
            }
        }

        public class JsonGetRespAsync : BaseIntegrationTest
        {
            [Fact]
            public async Task CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = (await _db.JsonGetRespAsync(key))[2];

                Assert.Equal("world", result.ToString());
            }
        }


        public class JsonIndexAddAsync : BaseIntegrationTest
        {
            [Fact(Skip = "This command has been deprecated and will be removed in a future version of RedisJson.")]
            public async Task CanExecute()
            {
                var index = Guid.NewGuid().ToString();

                var result = await _db.JsonIndexAddAsync(index, "test_field", "$.a");

                Assert.True(result);
                Assert.Equal("OK", result.RawResult);
            }
        }

        public class JsonIndexDeleteAsync : BaseIntegrationTest
        {
            [Fact(Skip = "This command has been deprecated and will be removed in a future version of RedisJson.")]
            public async Task CanExecute()
            {
                var index = Guid.NewGuid().ToString();

                await _db.JsonIndexAddAsync(index, "some_field", "$.a");

                var result = await _db.JsonIndexDeleteAsync(index);

                Assert.True(result);
                Assert.Equal("OK", result.RawResult);
            }
        }

        public class JsonIndexGetAsync : BaseIntegrationTest
        {
            [Fact(Skip = "This command has been deprecated and will be removed in a future version of RedisJson.")]
            public async Task CanExecute()
            {
                var index = Guid.NewGuid().ToString().Substring(0, 4);
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync($"{key}_1", "{\"last\":\"Joe\", \"first\":\"Mc\"}", index: index);
                await _db.JsonSetAsync($"{key}_2", "{\"last\":\"Joan\", \"first\":\"Mc\"}", index: index);

                await _db.JsonIndexAddAsync(index, "last", "$.last");

                var result = (await _db.JsonIndexGetAsync(index, "Jo*")).ToString();

                Assert.Contains("Joe", result);
                Assert.Contains("Joan", result);
            }

            [Fact(Skip = "This command has been deprecated and will be removed in a future version of RedisJson.")]
            public async Task CanExecuteWithSerializer()
            {
                var index = Guid.NewGuid().ToString().Substring(0, 4);
                var key = Guid.NewGuid().ToString();

                await _db.JsonSetAsync($"{key}_1", "{\"last\":\"Joe\", \"first\":\"Mc\"}", index: index);
                await _db.JsonSetAsync($"{key}_2", "{\"last\":\"Joan\", \"first\":\"Mc\"}", index: index);

                await _db.JsonIndexAddAsync(index, "last", "$.last");

                var result = await _db.JsonIndexGetAsync<ExamplePerson>(index, "Jo*");

                Assert.Equal("Joe", result[$"{key}_1"].First().LastName);
                Assert.Equal("Joan", result[$"{key}_2"].First().LastName);
            }
        }
    }
}