using Xunit;

namespace NReJSON.Tests
{
    public class DatabaseExtensionsTests
    {
        public class JsonDelete
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonDelete("fake_key", ".fakeObject");

                Assert.Equal(new[] { "JSON.DEL", "fake_key", ".fakeObject" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                db.JsonDelete("fake_key");

                Assert.Equal(new[] { "JSON.DEL", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonGet
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonGet("fake_key", ".firstPath", ".firstPath.secondItem");

                Assert.Equal(new[] { "JSON.GET", "fake_key", "NOESCAPE", ".firstPath", ".firstPath.secondItem" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                db.JsonGet("fake_key");

                Assert.Equal(new[] { "JSON.GET", "fake_key", "NOESCAPE", "." }, db.PreviousCommand);
            }

            [Fact]
            public void CanSpecifyEscaping()
            {
                var db = new FakeDatabase();

                db.JsonGet("fake_key", false);

                Assert.Equal(new[] { "JSON.GET", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonMultiGet
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase(true);

                db.JsonMultiGet(new[] { "fake_key::1", "fake_key::2" }, ".super.fake.path");

                Assert.Equal(new[] { "JSON.MGET", "fake_key::1", "fake_key::2", ".super.fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase(true);

                db.JsonMultiGet(new[] { "fake_key::1", "fake_key::2" });

                Assert.Equal(new[] { "JSON.MGET", "fake_key::1", "fake_key::2", "." }, db.PreviousCommand);
            }
        }

        public class JsonSet
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonSet("fake_key", "{\"hello\":\"world\"}", ".fake");

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".fake", "{\"hello\":\"world\"}" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                db.JsonSet("fake_key", "{\"hello\":\"world\"}");

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}" }, db.PreviousCommand);
            }

            [Fact]
            public void SetIfNotExistsIsProperlyEmitted()
            {
                var db = new FakeDatabase();

                db.JsonSet("fake_key", "{\"hello\":\"world\"}", setOption: SetOption.SetIfNotExists);

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "NX" }, db.PreviousCommand);
            }

            [Fact]
            public void SetOnlyIfExistsIsProperlyEmitted()
            {
                var db = new FakeDatabase();

                db.JsonSet("fake_key", "{\"hello\":\"world\"}", setOption: SetOption.SetOnlyIfExists);

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "XX" }, db.PreviousCommand);
            }
        }

        public class JsonType
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonType("fake_key", ".fakePath");

                Assert.Equal(new[] { "JSON.TYPE", "fake_key", ".fakePath" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                db.JsonType("fake_key");

                Assert.Equal(new[] { "JSON.TYPE", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonIncrementNumber
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonIncrementNumber("fake_key", ".fake.path", 2);

                Assert.Equal(new[] { "JSON.NUMINCRBY", "fake_key", ".fake.path", "2" }, db.PreviousCommand);
            }
        }

        public class JsonMultiplyNumber
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonMultiplyNumber("fake_key", ".fake.path", 5);

                Assert.Equal(new[] { "JSON.NUMMULTBY", "fake_key", ".fake.path", "5" }, db.PreviousCommand);
            }
        }

        public class JsonAppendJsonString
        {
            // TODO: Complete this once I figure out how this command is supposed to work.
        }

        public class JsonStringLength
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonStringLength("fake_key", ".fake.path");

                Assert.Equal(new[] { "JSON.STRLEN", "fake_key", ".fake.path" }, db.PreviousCommand);
            }

        }

        public class JsonArrayAppend
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonArrayAppend("fake_key", ".fake.path", "\"1\"", "\"2\"");

                Assert.Equal(new[] { "JSON.ARRAPPEND", "fake_key", ".fake.path", "\"1\"", "\"2\"" }, db.PreviousCommand);
            }
        }

        public class JsonArrayIndexOf
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonArrayIndexOf("fake_key", ".fake.path", "\"hello world\"", 10, 20);

                Assert.Equal(new[] { "JSON.ARRINDEX", "fake_key", ".fake.path", "\"hello world\"", "10", "20" }, db.PreviousCommand);
            }

            [Fact]
            public void HasZeroAsDefaultForStartAndStop()
            {
                var db = new FakeDatabase();

                db.JsonArrayIndexOf("fake_key", ".fake.path", "\"hello world\"");

                Assert.Equal(new[] { "JSON.ARRINDEX", "fake_key", ".fake.path", "\"hello world\"", "0", "0" }, db.PreviousCommand);
            }
        }

        public class JsonArrayInsert
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonArrayInsert("fake_key", ".fake.path", 15, "\"hello\"", "\"world\"");

                Assert.Equal(new[] { "JSON.ARRINSERT", "fake_key", ".fake.path", "15", "\"hello\"", "\"world\"" }, db.PreviousCommand);
            }
        }

        public class JsonArrayLength
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonArrayLength("fake_key", ".fake.array.path");

                Assert.Equal(new[] { "JSON.ARRLEN", "fake_key", ".fake.array.path" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                db.JsonArrayLength("fake_key");

                Assert.Equal(new[] { "JSON.ARRLEN", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonArrayPop
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonArrayPop("fake_key", ".what.ever", 10);

                Assert.Equal(new[] { "JSON.ARRPOP", "fake_key", ".what.ever", "10" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                db.JsonArrayPop("fake_key", index: 10);

                Assert.Equal(new[] { "JSON.ARRPOP", "fake_key", ".", "10" }, db.PreviousCommand);
            }

            [Fact]
            public void HasNegativeOneAsDefaultIndex()
            {
                var db = new FakeDatabase();

                db.JsonArrayPop("fake_key", ".what.ever");

                Assert.Equal(new[] { "JSON.ARRPOP", "fake_key", ".what.ever", "-1" }, db.PreviousCommand);
            }
        }

        public class JsonArrayTrim
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonArrayTrim("fake_key", ".fake.path", 1, 10);

                Assert.Equal(new[] { "JSON.ARRTRIM", "fake_key", ".fake.path", "1", "10" }, db.PreviousCommand);
            }
        }

        public class JsonObjectKeys
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase(true);

                db.JsonObjectKeys("fake_key", ".fake.path");

                Assert.Equal(new[] { "JSON.OBJKEYS", "fake_key", ".fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase(true);

                db.JsonObjectKeys("fake_key");

                Assert.Equal(new[] { "JSON.OBJKEYS", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonObjectLength
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonObjectLength("fake_key", ".fake.path");

                Assert.Equal(new[] { "JSON.OBJLEN", "fake_key", ".fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                db.JsonObjectLength("fake_key");

                Assert.Equal(new[] { "JSON.OBJLEN", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonDebugMemory
        {
            [Fact]
            public void EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                db.JsonDebugMemory("fake_key", ".fake.path");

                Assert.Equal(new[] { "JSON.DEBUG", "MEMORY", "fake_key", ".fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public void HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                db.JsonDebugMemory("fake_key");

                Assert.Equal(new[] { "JSON.DEBUG", "MEMORY", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonGetResp
        {

        }
    }
}
