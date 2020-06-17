using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace NReJSON
{
    public static partial class DatabaseExtensions
    {
        private static string[] CombineArguments(params object[] args)
        {
            IEnumerable<string> _combineArguments(object[] _args)
            {
                if (args == null)
                {
                    yield break;
                }

                foreach (var arg in args)
                {
                    if (arg.GetType() == typeof(RedisKey[]))
                    {
                        foreach (var aa in (RedisKey[])arg)
                        {
                            yield return aa.ToString();
                        }
                    }
                    else if (arg.GetType().IsArray)
                    {
                        foreach (var aa in (object[])arg)
                        {
                            yield return aa.ToString();
                        }
                    }
                    else
                    {
                        yield return arg.ToString();
                    }
                }
            }

            return _combineArguments(args).Where(a => a.Length > 0).ToArray();
        }

        private static string[] PathsOrDefault(string[] paths, string[] @default) =>
            paths == null || paths.Length == 0 ? @default : paths;

        private static string GetSetOptionString(SetOption setOption)
        {
            switch (setOption)
            {
                case SetOption.Default:
                    return string.Empty;
                case SetOption.SetIfNotExists:
                    return "NX";
                case SetOption.SetOnlyIfExists:
                    return "XX";
                default:
                    return string.Empty;
            }
        }

        private static string[] ResolveIndexSpecification(string index)
        {
            if (string.IsNullOrEmpty(index))
            {
                return new[] { string.Empty };
            }
            else
            {
                return new[] { "INDEX", index };
            }
        }
    }
}
