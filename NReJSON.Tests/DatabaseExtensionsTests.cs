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

        }

        public class JsonType
        {

        }

        public class JsonIncrementNumber
        {

        }

        public class JsonMultiplyNumber
        {

        }

        public class JsonAppendJsonString
        {

        }

        public class JsonStringLength
        {

        }

        public class JsonArrayAppend
        {

        }

        public class JsonArrayIndexOf
        {

        }

        public class JsonArrayInsert
        {

        }

        public class JsonArrayLength
        {

        }

        public class JsonArrayPop
        {

        }

        public class JsonArrayTrim
        {

        }

        public class JsonObjectKeys
        {

        }

        public class JsonObjectLength
        {

        }

        public class JsonDebugMemory
        {

        }

        public class JsonForget
        {

        }

        public class JsonGetResp
        {

        }
    }
}
