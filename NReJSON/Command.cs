using System.Linq;
using System.Text;
using StackExchange.Redis;

namespace NReJSON
{
    internal class Command
    {
        private const string EmptySpace = " ";
        private readonly Commands.Json _jsonCommandType;
        private readonly RedisKey[] _keys;
        private readonly string[] _arguments;

        internal Command(Commands.Json jsonCommandType, RedisKey[] keys, params string[] arguments)
        {
            _jsonCommandType = jsonCommandType;
            _keys = keys;
            _arguments = arguments;
        }

        private string GenerateCommandString()
        {
            var command = new StringBuilder();

            command.Append(nameof(Commands.Json).ToUpperInvariant());
            command.Append(".");
            command.Append(_jsonCommandType.ToString());
            command.Append(EmptySpace);
            command.Append(string.Join(EmptySpace, _keys));

            if (_arguments?.Any() ?? false)
            {
                command.Append(EmptySpace);
                command.Append(string.Join(EmptySpace, _arguments));
            }

            return command.ToString();
        }

        public override string ToString() => GenerateCommandString();
    }
}