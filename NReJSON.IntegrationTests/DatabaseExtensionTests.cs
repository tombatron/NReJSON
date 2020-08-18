using NReJSON.IntegrationTests.Models;
using StackExchange.Redis;
using System;
using System.Linq;
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

                Assert.True(result);
            }

            [Fact]
            public void CanExecuteWithSerializer()
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

                var result = _db.JsonSet(key, obj);
                
                Assert.True(result);
            }
        }

        public class JsonGet : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGet(key);

                Assert.False(result.IsNull);
            }

            [Fact]
            public void CanExecuteWithIdent()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGet(key, indent: "&");

                Assert.Contains("&", (string)result);
            }

            [Fact]
            public void CanExecuteWithNewline()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGet(key, newline: "=");

                Assert.Contains("=", (string)result);
            }

            [Fact]
            public void CanExecuteWithSpace()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGet(key, space: "+");

                Assert.Contains("+", (string)result);
            }

            [Fact]
            public void CanExecuteWithSerializer()

            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGet<ExampleHelloWorld>(key);

                Assert.NotNull(result);
                Assert.Equal("world", result.Hello);
                Assert.Equal("moon", result.GoodNight.Value);
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

            [Fact]
            public void CanExecuteWithSerializer()
            {
                var key1 = Guid.NewGuid().ToString("N");
                var key2 = Guid.NewGuid().ToString("N");

                _db.JsonSet(key1, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");
                _db.JsonSet(key2, "{\"hello\": \"tom\", \"goodnight\": {\"value\": \"tom\"}}");

                var result = _db.JsonMultiGet<ExampleHelloWorld>(new RedisKey[] { key1, "say what?", key2 }).ToList();

                Assert.Equal(3, result.Count);
                Assert.Null(result[1]);
                Assert.Contains("world", result[0].Hello);
                Assert.Contains("tom", result[2].Hello);
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
            [Fact]
            public void CanExecuteAsync()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\":\"world\"}");

                var result = _db.JsonAppendJsonString(key, ".hello", "\"!\"");

                Assert.Equal(6, result);
            }

            [Fact]
            public void WillAppendProvidedJsonStringIntoExistingJsonString()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\":\"world\"}");

                _db.JsonAppendJsonString(key, ".hello", "\"!\"");

                var helloValue = _db.JsonGet<string>(key, ".hello");

                Assert.Equal("world!", helloValue);
            }
            
            [Fact]
            public void WillApendProvidedJsonStringIntoRootIfNoPathProvided()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "\"world\"");

                _db.JsonAppendJsonString(key, jsonString: "\"!\"");

                var helloValue = _db.JsonGet<string>(key);

                Assert.Equal("world!", helloValue);
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

            [Fact]
            public void WillReturnNullIfPathDoesntExist()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\":\"world\"}");

                var result = _db.JsonStringLength("doesnt_exist", ".hello.doesnt.exist");

                Assert.Null(result);
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

            [Fact]
            public void CanExecuteWithSerializer()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = _db.JsonArrayPop<string>(key, ".array", 1);

                Assert.Equal("world", result);
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

                Assert.True(result > 50);
            }
        }

        public class JsonGetResp : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGetResp(key)[2];

                Assert.Equal("world", result.ToString());
            }
        }

        public class JsonIndexAdd : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var index = Guid.NewGuid().ToString();

                var result = _db.JsonIndexAdd(index, "test_field", "$.a");

                Assert.True(result);
                Assert.Equal("OK", result.RawResult);
            }
        }

        public class JsonIndexDelete : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var index = Guid.NewGuid().ToString();

                _db.JsonIndexAdd(index, "some_field", "$.a");

                var result = _db.JsonIndexDelete(index);

                Assert.True(result);
                Assert.Equal("OK", result.RawResult);
            }
        }

        public class JsonIndexGet : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var index = Guid.NewGuid().ToString().Substring(0, 4);
                var key = Guid.NewGuid().ToString();

                _db.JsonSet($"{key}_1", "{\"last\":\"Joe\", \"first\":\"Mc\"}", index: index);
                _db.JsonSet($"{key}_2", "{\"last\":\"Joan\", \"first\":\"Mc\"}", index: index);

                _db.JsonIndexAdd(index, "last", "$.last");

                var result = _db.JsonIndexGet(index, "Jo*").ToString();

                Assert.Contains("Joe", result);
                Assert.Contains("Joan", result);
            }

            [Fact]
            public void CanExecuteWithSerializer()
            {
                var index = Guid.NewGuid().ToString().Substring(0, 4);
                var key = Guid.NewGuid().ToString();

                _db.JsonSet($"{key}_1", "{\"last\":\"Joe\", \"first\":\"Mc\"}", index: index);
                _db.JsonSet($"{key}_2", "{\"last\":\"Joan\", \"first\":\"Mc\"}", index: index);

                _db.JsonIndexAdd(index, "last", "$.last");

                var result = _db.JsonIndexGet<ExamplePerson>(index, "Jo*");

                Assert.Equal("Joe", result[$"{key}_1"].First().LastName);
                Assert.Equal("Joan", result[$"{key}_2"].First().LastName);
            }            
        }
    }
}