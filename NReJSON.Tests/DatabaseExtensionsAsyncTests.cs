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

        public class JsonGetAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key", ".firstPath", ".firstPath.secondItem");

                Assert.Equal(new[] { "JSON.GET", "fake_key", "NOESCAPE", ".firstPath", ".firstPath.secondItem" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key");

                Assert.Equal(new[] { "JSON.GET", "fake_key", "NOESCAPE", "." }, db.PreviousCommand);
            }

            [Fact]
            public async Task CanSpecifyEscaping()
            {
                var db = new FakeDatabase();

                await db.JsonGetAsync("fake_key", false);

                Assert.Equal(new[] { "JSON.GET", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonMultiGetAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase(true);

                await db.JsonMultiGetAsync(new[] { "fake_key::1", "fake_key::2" }, ".super.fake.path");

                Assert.Equal(new[] { "JSON.MGET", "fake_key::1", "fake_key::2", ".super.fake.path" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase(true);

                await db.JsonMultiGetAsync(new[] { "fake_key::1", "fake_key::2" });

                Assert.Equal(new[] { "JSON.MGET", "fake_key::1", "fake_key::2", "." }, db.PreviousCommand);
            }
        }

        public class JsonSetAsync
        {

        }

        public class JsonTypeAsync
        {

        }

        public class JsonIncrementNumberAsync
        {

        }

        public class JsonMultiplyNumberAsync
        {

        }

        public class JsonAppendJsonStringAsync
        {

        }

        public class JsonStringLengthAsync
        {

        }

        public class JsonArrayAppendAsync
        {

        }

        public class JsonArrayIndexOfAsync
        {

        }

        public class JsonArrayInsertAsync
        {

        }

        public class JsonArrayLengthAsync
        {

        }

        public class JsonArrayPopAsync
        {

        }

        public class JsonArrayTrimAsync
        {

        }

        public class JsonObjectKeysAsync
        {

        }

        public class JsonObjectLengthAsync
        {

        }

        public class JsonDebugMemoryAsync
        {

        }

        public class JsonForgetAsync
        {

        }

        public class JsonGetRespAsync
        {

        }
    }
}
