using NReJSON.Commands;
using StackExchange.Redis;
using Xunit;

namespace NReJSON.Tests.Commands
{
    public class CommandTests
    {
        internal class TestCommand : BaseCommand
        {
            internal TestCommand(CommandType.Json jsonCommandType, RedisKey[] keys, params string[] arguments) : base(jsonCommandType, keys, arguments)
            {
            }
        }

        [Fact]
        public void StringRepresentationIsCorrect()
        {
            var command = new TestCommand(CommandType.Json.DEL, new RedisKey[] { "First_Key", "Second_Key" }, "first_argument", "second_argument");

            Assert.Equal("JSON.DEL First_Key Second_Key first_argument second_argument", command.ToString());
        }
    }
}