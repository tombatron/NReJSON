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
    }
}
