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
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", ".fake");

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".fake", "{\"hello\":\"world\"}" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}");

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}" }, db.PreviousCommand);
            }

            [Fact]
            public async Task SetIfNotExistsIsProperlyEmitted()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", setOption: SetOption.SetIfNotExists);

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "NX" }, db.PreviousCommand);
            }

            [Fact]
            public async Task SetOnlyIfExistsIsProperlyEmitted()
            {
                var db = new FakeDatabase();

                await db.JsonSetAsync("fake_key", "{\"hello\":\"world\"}", setOption: SetOption.SetOnlyIfExists);

                Assert.Equal(new[] { "JSON.SET", "fake_key", ".", "{\"hello\":\"world\"}", "XX" }, db.PreviousCommand);
            }
        }

        public class JsonTypeAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonTypeAsync("fake_key", ".fakePath");

                Assert.Equal(new[] { "JSON.TYPE", "fake_key", ".fakePath" }, db.PreviousCommand);
            }

            [Fact]
            public async Task HasRootAsDefaultPath()
            {
                var db = new FakeDatabase();

                await db.JsonTypeAsync("fake_key");

                Assert.Equal(new[] { "JSON.TYPE", "fake_key", "." }, db.PreviousCommand);
            }
        }

        public class JsonIncrementNumberAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonIncrementNumberAsync("fake_key", ".fake.path", 2);

                Assert.Equal(new[] { "JSON.NUMINCRBY", "fake_key", ".fake.path", "2" }, db.PreviousCommand);
            }
        }

        public class JsonMultiplyNumberAsync
        {
            [Fact]
            public async Task EmitsCorrectParameters()
            {
                var db = new FakeDatabase();

                await db.JsonMultiplyNumberAsync("fake_key", ".fake.path", 5);

                Assert.Equal(new[] { "JSON.NUMMULTBY", "fake_key", ".fake.path", "5" }, db.PreviousCommand);
            }
        }

        public class JsonAppendJsonStringAsync
        {
            // TODO: Complete this once I figure out how this command is supposed to work.
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
