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
    }
}
