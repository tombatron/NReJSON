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

                Assert.Contains("&", (string) result);
            }

            [Fact]
            public void CanExecuteWithNewline()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGet(key, newline: "=");

                Assert.Contains("=", (string) result);
            }

            [Fact]
            public void CanExecuteWithSpace()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                var result = _db.JsonGet(key, space: "+");

                Assert.Contains("+", (string) result);
            }

            [Fact]
            public void CanExecuteWithSerializer()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"value\": \"moon\"}}");

                ExampleHelloWorld result = _db.JsonGet<ExampleHelloWorld>(key);

                Assert.NotNull(result);
                Assert.Equal("world", result.Hello);
                Assert.Equal("moon", result.GoodNight.Value);
            }

            [Fact]
            public void CanExecuteWithMultiplePathMatches()
            {
                var key = Guid.NewGuid().ToString("N");
                
                _db.JsonSet(key, "{\"hello\": \"world\", \"goodnight\": {\"hello\": \"moon\"}}");

                var result = _db.JsonGet(key, paths: "$..hello");
                
                Assert.Equal("[\"world\",\"moon\"]", result.ToString());
            }

            [Fact]
            public void CanExecuteWithMultiplePathMatchesWithSerializer()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\r\n\"object\":{\r\n\"hello\":\"world\",\r\n\"goodnight\":{\r\n\"value\":\"moon\"\r\n}\r\n},\r\n\"nested\":{\r\n\"object\":{\r\n\"hello\": \"guy\",\r\n\"goodnight\": {\r\n\"value\": \"stuff\"\r\n}\r\n}\r\n}\r\n}");

                var result = _db.JsonGet<ExampleHelloWorld>(key, paths: "$..object").ToList();

                var firstResult = result[0];
                var secondResult = result[1];
                
                Assert.Equal("world", firstResult.Hello);
                Assert.Equal("guy", secondResult.Hello);
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

                var result = _db.JsonMultiGet(new RedisKey[] {key1, key2});

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

                var result = _db.JsonMultiGet<ExampleHelloWorld>(new RedisKey[] {key1, "say what?", key2}).ToList();

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

                Assert.Equal(expectedResult, (double) result, 2);
            }

            [Fact]
            public void CanExecuteOnMultiplePaths()
            {
                var key = Guid.NewGuid().ToString("N");
                
                _db.JsonSet(key, "{\"a\":1,\"number\":{\"a\":2}}");

                var result = _db.JsonIncrementNumber(key, "$..a", 2).ToList();
                
                Assert.Equal(3, result.First());
                Assert.Equal(4, result[1]);
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

                Assert.Equal(expectedResult, (double) result, 2);
            }
            
            [Fact]
            public void CanExecuteOnMultiplePaths()
            {
                var key = Guid.NewGuid().ToString("N");
                
                _db.JsonSet(key, "{\"a\":1,\"number\":{\"a\":2}}");

                var result = _db.JsonMultiplyNumber(key, "$..a", 2).ToList();
                
                Assert.Equal(2, result.First());
                Assert.Equal(4, result[1]);
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

                Assert.Equal(6, result[0].Value);
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
            public void WillAppendProvidedJsonStringIntoRootIfNoPathProvided()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "\"world\"");

                _db.JsonAppendJsonString(key, jsonString: "\"!\"");

                var helloValue = _db.JsonGet<string>(key);

                Assert.Equal("world!", helloValue);
            }
            
            [Fact]
            public void CanAppendOnMultiplePaths()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key,
                    "{\"a\":\"foo\", \"nested\": {\"a\": \"hello\"}, \"nested2\": {\"a\": 31}}");

                var result = _db.JsonAppendJsonString(key, "$..a", "\"baz\"");

                Assert.Equal(3, result.Length);
                Assert.Equal(6, result[0]);
                Assert.Equal(8, result[1]);
                Assert.Null(result[2]);
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

                Assert.Equal(5, result[0]);
            }

            [Fact]
            public void WillReturnNullIfPathDoesntExist()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"hello\":\"world\"}");

                var result = _db.JsonStringLength("doesnt_exist", ".hello.doesnt.exist");

                Assert.Null(result);
            }

            [Fact]
            public void CanExecuteOnMultiplePaths()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"a\":\"foo\", \"nested\": {\"a\": \"hello\"}, \"nested2\": {\"a\": 31}}");

                var result = _db.JsonStringLength(key, "$..a");

                Assert.Equal(3, result.Length);
                Assert.Equal(3, result[0]);
                Assert.Equal(5, result[1]);
                Assert.Null(result[2]);
            }
        }

        public class JsonArrayAppend : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"array\": []}");

                var result = _db.JsonArrayAppend(key, ".array", "\"hello\"", "\"world\"", 2);

                Assert.Equal(3, result[0]);
            }

            [Fact]
            public void CanExecuteOnMultipleMatchingPaths()
            {
                var key = Guid.NewGuid().ToString("N");

                _db.JsonSet(key, "{\"a\":[1], \"nested\": {\"a\": [1,2]}, \"nested2\": {\"a\": 42}}");

                var result = _db.JsonArrayAppend(key, "$..a", 3, 4);
                
                Assert.Equal(3, result.Length);
                
                Assert.Equal(3, result[0]);
                Assert.Equal(4, result[1]);
                Assert.Null(result[2]);
            }
        }

        public class JsonArrayIndexOf : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\", 2]}");

                var result = _db.JsonArrayIndexOf(key, ".array", "\"world\"", 0, 2);

                Assert.Equal(1, result[0]);
                
                result = _db.JsonArrayIndexOf(key, ".array", 2, 0, 4);
                
                Assert.Equal(3, result[0]);
            }
            
            [Fact]
            public void CanExecuteOnMultipleMatchingPaths()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[1,2,3,2], \"nested\": {\"a\": [3,4]}}");

                var result = _db.JsonArrayIndexOf(key, "$..a", 2);
                
                Assert.Equal(2, result.Length);
                
                Assert.Equal(1, result[0]);
                Assert.Equal(-1, result[1]);
            }            
        }

        public class JsonArrayInsert : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = _db.JsonArrayInsert(key, ".array", 1, "\"there\"", 2);

                Assert.Equal(5, result[0]);
            }

            [Fact]
            public void CanExecuteOnMultipleMatchingPaths()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[3], \"nested\": {\"a\": [3,4]}}");

                var result = _db.JsonArrayInsert(key, "$..a", 0, 1, 2);
                
                Assert.Equal(2, result.Length);
                
                Assert.Equal(3, result[0]);
                Assert.Equal(4, result[1]);
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

                Assert.Equal(3, result[0]);
            }
            
            [Fact]
            public void CanExecuteOnMultipleMatchingPaths()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[3], \"nested\": {\"a\": [3,4]}}");

                var result = _db.JsonArrayLength(key, "$..a");
                
                Assert.Equal(2, result.Length);
                
                Assert.Equal(1, result[0]);
                Assert.Equal(2, result[1]);
            }
            
            [Fact]
            public void CanExecuteOnMultipleMatchingPathsWithOneNonArrayMatch()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[1,2,3,2], \"nested\": {\"a\": false}}");

                var result = _db.JsonArrayLength(key, "$..a");
                
                Assert.Equal(2, result.Length);
                
                Assert.Equal(4, result[0]);
                Assert.Null(result[1]);
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

                Assert.Equal("\"world\"", result[0]);
            }

            [Fact]
            public void CanExecuteWithSerializer()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"array\": [\"hi\", \"world\", \"!\"]}");

                var result = _db.JsonArrayPop<string>(key, ".array", 1);

                Assert.Equal("world", result[0]);
            }

            [Fact]
            public void CanExecuteOnMultipleMatchingPaths()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[3], \"nested\": {\"a\": [3,4]}}");

                var result = _db.JsonArrayPop(key, "$..a");
                
                Assert.Equal(2, result.Length);
                
                Assert.Equal("3", result[0]);
                Assert.Equal("4", result[1]);
            }
            
            [Fact]
            public void CanExecuteOnMultipleMatchingPathsWithNulls()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[\"foo\", \"bar\"], \"nested\": {\"a\": false}, \"nested2\": {\"a\":[]}}");

                var result = _db.JsonArrayPop(key, "$..a");
                
                Assert.Equal(3, result.Length);
                
                Assert.Equal("\"bar\"", result[0]);
                Assert.Null(result[1]);
                Assert.Null(result[2]);
            }               
            
            [Fact]
            public void CanExecuteOnMultipleMatchingPathsWithSerializer()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[3], \"nested\": {\"a\": [3,4]}}");

                var result = _db.JsonArrayPop<int?>(key, "$..a");
                
                Assert.Equal(2, result.Length);
                
                Assert.Equal(3, result[0]);
                Assert.Equal(4, result[1]);
            } 
            
            [Fact]
            public void CanExecuteOnMultipleMatchingPathsWithSerializerWithNulls()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[\"foo\", \"bar\"], \"nested\": {\"a\": false}, \"nested2\": {\"a\":[]}}");

                var result = _db.JsonArrayPop<string>(key, "$..a");
                
                Assert.Equal(3, result.Length);
                
                Assert.Equal("bar", result[0]);
                Assert.Null(result[1]);
                Assert.Null(result[2]);
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

                Assert.Equal(2, result[0]);
            }
            
            [Fact]
            public void CanExecuteForMultiplePaths()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"a\":[1,2,3,2], \"nested\": {\"a\": false}}");

                var result = _db.JsonArrayTrim(key, "$..a", 1, 1);
                
                Assert.Equal(2, result.Length);
                
                Assert.Equal(1, result[0]);
                Assert.Null(result[1]);
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

                Assert.Equal(new[] {"hello", "goodnight"}, result.Select(x => x.ToString()).ToArray());
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

        public class JsonToggle : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"foo\":true}");

                Assert.False(_db.JsonToggle(key, ".foo"));
                Assert.True(_db.JsonToggle(key, ".foo"));
            }
        }

        public class JsonClear : BaseIntegrationTest
        {
            [Fact]
            public void CanExecute()
            {
                var key = Guid.NewGuid().ToString();

                _db.JsonSet(key, "{\"foo\":[1,2,3,4]}");

                Assert.Equal(1, _db.JsonClear(key, ".foo"));
            }
        }
    }
}