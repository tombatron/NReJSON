using System.Text;

namespace NReJSON
{
    internal class Command
    {
        private readonly Commands.Json _jsonCommandType;
        private readonly StringBuilder _commandString = new StringBuilder();

        internal Command(Commands.Json jsonCommandType)
        {
            _jsonCommandType = jsonCommandType;

            _commandString.Append(nameof(Commands.Json).ToUpperInvariant());
            _commandString.Append(".");
            _commandString.Append(_jsonCommandType.ToString());
        }

        public override string ToString() => _commandString.ToString();
    }
}