using StackExchange.Redis;

namespace NReJSON.Commands
{
    internal sealed class Get : BaseCommand
    {
        internal Get(RedisKey key, params string[] paths) : 
            base(CommandType.Json.GET, new [] { key }, paths) { }
    }
}