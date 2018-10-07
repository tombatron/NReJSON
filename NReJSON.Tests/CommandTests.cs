using Xunit;

namespace NReJSON.Tests
{
    public class CommandTests
    {
        [Fact]
        public void StringRepresentationIsCorrect()
        {
            var command = new Command(Commands.Json.DEL);

            Assert.Equal("JSON.DEL", command.ToString());
        }        
    }
}