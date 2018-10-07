namespace NReJSON
{
    internal class Command
    {
        private readonly Commands.Json _jsonCommandType;

        internal Command(Commands.Json jsonCommandType)
        {
            _jsonCommandType = jsonCommandType;
        }

        public override string ToString()
        {
            return "Whatever.";
        }
    }
}