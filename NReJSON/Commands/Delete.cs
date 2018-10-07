using StackExchange.Redis;

namespace NReJSON.Commands
{
    internal sealed class Delete : BaseCommand
    {
        internal Delete(RedisKey key, string path = ""):
            base(CommandType.Json.DEL, new [] { key }, path == string.Empty ? null : new [] { path }) { }
    }
}