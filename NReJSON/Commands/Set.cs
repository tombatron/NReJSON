using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace NReJSON.Commands
{
    internal sealed class Set : BaseCommand
    {
        internal Set(RedisKey key, string json, SetOption setOption = SetOption.Default):
            base(CommandType.Json.SET, new [] { key }, CreateArguments(json, setOption)) { }

        private static string[] CreateArguments(string json, SetOption setOption)
        {
            IEnumerable<string> result()
            {
                yield return json;

                switch (setOption)
                {
                    case SetOption.Default:
                        break;
                    case SetOption.SetIfNotExists:
                        yield return "NX";

                        break;
                    case SetOption.SetOnlyIfExists:
                        yield return "XX";

                        break;
                    default:
                        break;
                }
            };

            return result().ToArray();
        }
    }
}