using Xunit;

namespace NReJSON.IntegrationTests
{
    public class DatabaseExtensionTests : BaseIntegrationTest
    {
        [Fact]
        public void ItCanSetJson()
        {
            var result = _db.JsonSet("test_key_set", "{}");

            Assert.NotNull(result);
        }
    }
}
