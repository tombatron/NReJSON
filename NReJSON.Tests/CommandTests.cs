using StackExchange.Redis;
using Xunit;

namespace NReJSON.Tests
{
    public class CommandTests
    {
        [Fact]
        public void StringRepresentationIsCorrect()
        {
            var command = new Command(Commands.Json.DEL, new RedisKey[] { "First_Key", "Second_Key" }, "first_argument", "second_argument");

            Assert.Equal("JSON.DEL First_Key Second_Key first_argument second_argument", command.ToString());
        }
    }
}