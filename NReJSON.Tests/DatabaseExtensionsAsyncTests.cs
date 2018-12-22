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

                Assert.Equal(new[] { "JSON.DEL", "fake_key", ".fakeObject" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonDeleteAsync("fake_key");

                Assert.Equal(new[] { "JSON.DEL", "fake_key", "." }, db.PreviousCommand);
            }
        }
    }
}
