using System.Linq;
using System.Text;
using StackExchange.Redis;

namespace NReJSON.Commands
{
    internal abstract class BaseCommand
    {
        private const string EmptySpace = " ";
        private readonly CommandType.Json _jsonCommandType;
        private readonly RedisKey[] _keys;
        private readonly string[] _arguments;

        internal BaseCommand(CommandType.Json jsonCommandType, RedisKey[] keys, params string[] arguments)
        {
            _jsonCommandType = jsonCommandType;
            _keys = keys;
            _arguments = arguments;
        }

        private string GenerateCommandString()
        {
            var command = new StringBuilder();

            command.Append(nameof(CommandType.Json).ToUpperInvariant());
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