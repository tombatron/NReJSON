using System.Threading.Tasks;
using Xunit;

namespace NReJSON.Tests
{
    public class DatabaseExtensionsAsyncTests
    {
        public class JsonDeleteAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonDeleteAsync("fake_key", ".fakeObject");

                Assert.Equal(new [] { "JSON.DEL", "fake_key", ".fakeObject" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonDeleteAsync("fake_key");

                Assert.Equal(new [] { "JSON.DEL", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonGetAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key", ".firstPath", ".firstPath.secondItem");

                Assert.Equal(new [] { "JSON.GET", "fake_key", "NOESCAPE", ".firstPath", ".firstPath.secondItem" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key");

                Assert.Equal(new [] { "JSON.GET", "fake_key", "NOESCAPE", "." }, db.PreviousCommand);
            }

            [Fact]
            public async Task CanSpecifyEscaping()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key", false);

                Assert.Equal(new [] { "JSON.GET", "fake_key", "." }, db.PreviousCommand);
            }

            [Fact]
            public async Task CanSpecifyIndent()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key", indent: "\t\t");

                Assert.Equal(new [] { "JSON.GET", "fake_key", "INDENT", "\t\t", "." }, db.PreviousCommand);
            }

            [Fact]
            public async Task CanSpecifySpace()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key", space: "  ");

                Assert.Equal(new [] { "JSON.GET", "fake_key", "SPACE", "  ", "." }, db.PreviousCommand);
            }

            [Fact]
            public async Task CanSpecifyNewLine()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key", newline: "\r\n");

                Assert.Equal(new [] { "JSON.GET", "fake_key", "NEWLINE", "\r\n", "." }, db.PreviousCommand);
            }

            [Fact]
            public async Task CanSpecifyIndentSpaceAndNewLine()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key", indent: "\t", newline: "\r\n", space: "  ");

                Assert.Equal(new [] { "JSON.GET", "fake_key", "INDENT", "\t", "NEWLINE", "\r\n", "SPACE", "  ", "." }, db.PreviousCommand);
            }
        }

        public class JsonMultiGetAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase(true);

                await db.JsonMultiGetAsync(new [] { "fake_key::1", "fake_key::2" }, ".super.fake.path");

                Assert.Equal(new [] { "JSON.MGET", "fake_key::1", "fake_key::2", ".super.fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase(true);

                await db.JsonMultiGetAsync(new [] { "fake_key::1", "fake_key::2" });

                Assert.Equal(new [] { "JSON.MGET", "fake_key::1", "fake_key::2", "." }, db.PreviousCommand);
            }
        }

        public class JsonSetAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", ".fake");

                Assert.Equal(new [] { "JSON.SET", "fake_key", ".fake", "{\"hello\":\"world\"}" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}");

                Assert.Equal(new [] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}" }, db.PreviousCommand);
            }

            [Fact]
            public async Task SetIfNotExistsIsProperlyEmitted()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", setOption : SetOption.SetIfNotExists);

                Assert.Equal(new [] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "NX" }, db.PreviousCommand);
            }

            [Fact]
            public async Task SetOnlyIfExistsIsProperlyEmitted()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", setOption : SetOption.SetOnlyIfExists);

                Assert.Equal(new [] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "XX" }, db.PreviousCommand);
            }

            [Fact]
            public async Task SetIndexIsProperlyEmitted()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", index: "message");

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "INDEX", "message" }, db.PreviousCommand);
            }

            [Fact]
            public async Task SetIndexIsProperlyEmittedAfterSetOnlyIfExists()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", setOption: SetOption.SetOnlyIfExists, index: "message");

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "XX", "INDEX", "message" }, db.PreviousCommand);
            }

            [Fact]
            public async Task SetIndexIsProperlyEmittedAfterSetIfNotExists()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", setOption: SetOption.SetIfNotExists, index: "message");

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "NX", "INDEX", "message" }, db.PreviousCommand);
            }            
        }

        public class JsonTypeAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonTypeAsync("fake_key", ".fakePath");

                Assert.Equal(new [] { "JSON.TYPE", "fake_key", ".fakePath" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonTypeAsync("fake_key");

                Assert.Equal(new [] { "JSON.TYPE", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonIncrementNumberAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonIncrementNumberAsync("fake_key", ".fake.path", 2);

                Assert.Equal(new [] { "JSON.NUMINCRBY", "fake_key", ".fake.path", "2" }, db.PreviousCommand);
            }
        }

        public class JsonMultiplyNumberAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonMultiplyNumberAsync("fake_key", ".fake.path", 5);

                Assert.Equal(new [] { "JSON.NUMMULTBY", "fake_key", ".fake.path", "5" }, db.PreviousCommand);
            }
        }

        public class JsonAppendJsonStringAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonAppendJsonStringAsync("fake_key", ".fake.path", "\"fake_string\"");

                Assert.Equal(new[] { "JSON.STRAPPEND", "fake_key", ".fake.path", "\"fake_string\"" }, db.PreviousCommand);
            }
            
            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonAppendJsonStringAsync("fake_key", jsonString: "\"fake_string\"");

                Assert.Equal(new[] { "JSON.STRAPPEND", "fake_key", ".", "\"fake_string\"" }, db.PreviousCommand);
            }
        }

        public class JsonStringLengthAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonStringLengthAsync("fake_key", ".fake.path");

                Assert.Equal(new [] { "JSON.STRLEN", "fake_key", ".fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonStringLengthAsync("fake_key");

                Assert.Equal(new [] { "JSON.STRLEN", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonArrayAppendAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonArrayAppendAsync("fake_key", ".fake.path", "\"1\"", "\"2\"");

                Assert.Equal(new [] { "JSON.ARRAPPEND", "fake_key", ".fake.path", "\"1\"", "\"2\"" }, db.PreviousCommand);
            }
        }

        public class JsonArrayIndexOfAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonArrayIndexOfAsync("fake_key", ".fake.path", "\"hello world\"", 10, 20);

                Assert.Equal(new [] { "JSON.ARRINDEX", "fake_key", ".fake.path", "\"hello world\"", "10", "20" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasZeroAsDefaultForStartAndStop()
            {
                var db = new FakeDatabase();

                await db.JsonArrayIndexOfAsync("fake_key", ".fake.path", "\"hello world\"");

                Assert.Equal(new [] { "JSON.ARRINDEX", "fake_key", ".fake.path", "\"hello world\"", "0", "0" }, db.PreviousCommand);
            }
        }

        public class JsonArrayInsertAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonArrayInsertAsync("fake_key", ".fake.path", 15, "\"hello\"", "\"world\"");

                Assert.Equal(new [] { "JSON.ARRINSERT", "fake_key", ".fake.path", "15", "\"hello\"", "\"world\"" }, db.PreviousCommand);
            }
        }

        public class JsonArrayLengthAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonArrayLengthAsync("fake_key", ".fake.array.path");

                Assert.Equal(new [] { "JSON.ARRLEN", "fake_key", ".fake.array.path" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonArrayLengthAsync("fake_key");

                Assert.Equal(new [] { "JSON.ARRLEN", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonArrayPopAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonArrayPopAsync("fake_key", ".what.ever", 10);

                Assert.Equal(new [] { "JSON.ARRPOP", "fake_key", ".what.ever", "10" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonArrayPopAsync("fake_key", index : 10);

                Assert.Equal(new [] { "JSON.ARRPOP", "fake_key", ".", "10" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasNegativeOneAsDefaultIndex()
            {
                var db = new FakeDatabase();

                await db.JsonArrayPopAsync("fake_key", ".what.ever");

                Assert.Equal(new [] { "JSON.ARRPOP", "fake_key", ".what.ever", "-1" }, db.PreviousCommand);
            }
        }

        public class JsonArrayTrimAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonArrayTrimAsync("fake_key", ".fake.path", 1, 10);

                Assert.Equal(new [] { "JSON.ARRTRIM", "fake_key", ".fake.path", "1", "10" }, db.PreviousCommand);
            }
        }

        public class JsonObjectKeysAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase(true);

                await db.JsonObjectKeysAsync("fake_key", ".fake.path");

                Assert.Equal(new [] { "JSON.OBJKEYS", "fake_key", ".fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase(true);

                await db.JsonObjectKeysAsync("fake_key");

                Assert.Equal(new [] { "JSON.OBJKEYS", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonObjectLengthAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonObjectLengthAsync("fake_key", ".fake.path");

                Assert.Equal(new [] { "JSON.OBJLEN", "fake_key", ".fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonObjectLengthAsync("fake_key");

                Assert.Equal(new [] { "JSON.OBJLEN", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonDebugMemoryAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonDebugMemoryAsync("fake_key", ".fake.path");

                Assert.Equal(new [] { "JSON.DEBUG", "MEMORY", "fake_key", ".fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonDebugMemoryAsync("fake_key");

                Assert.Equal(new [] { "JSON.DEBUG", "MEMORY", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonGetRespAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase(true);

                await db.JsonGetRespAsync("fake_key", ".hello.fake");

                Assert.Equal(new [] { "JSON.RESP", "fake_key", ".hello.fake" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase(true);

                await db.JsonGetRespAsync("fake_key");

                Assert.Equal(new [] { "JSON.RESP", "fake_key", "." }, db.PreviousCommand);
            }
        }
    }
}