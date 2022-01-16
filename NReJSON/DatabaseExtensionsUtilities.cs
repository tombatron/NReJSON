using System;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using static NReJSON.NReJSONSerializer;

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
                        foreach (var aa in (RedisKey[]) arg)
                        {
                            yield return aa.ToString();
                        }
                    }
                    else if (arg.GetType().IsArray)
                    {
                        foreach (var aa in (object[]) arg)
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

        private static readonly string[] RootPathStringArray = {"."};

        private static int?[] NullableIntArrayFrom(RedisResult result)
        {
            if (result.IsNull)
            {
                return null;
            }
            
            switch (result.Type)
            {
                case ResultType.Integer:
                    return new int?[] {(int) result};
                case ResultType.MultiBulk:
                    var resultArray = (RedisResult[]) result;
                    return resultArray.Select(x => (int?) x).ToArray();
                default:
                    throw new ArgumentException(nameof(result), "Not sure how to handle this result.");
            }
        }

        private static string[] StringArrayFrom(RedisResult result)
        {
            if (result.IsNull)
            {
                return null;
            }

            switch (result.Type)
            {
                case ResultType.BulkString:
                    return new[] {(string) result};
                case ResultType.MultiBulk:
                    var resultArray = (RedisResult[]) result;
                    return resultArray.Select(x => (string) x).ToArray();
                default:
                    throw new ArgumentException(nameof(result), "Not sure how to handle this result.");
            }
        }

        private static TResult[] TypedArrayFrom<TResult>(RedisResult result)
        {
            if (result.IsNull)
            {
                return default;
            }

            switch (result.Type)
            {
                case ResultType.BulkString:
                    return new TResult[] { SerializerProxy.Deserialize<TResult>(result) };
                case ResultType.MultiBulk:
                    var resultArray = (RedisResult[]) result;
                    var typedResult = new TResult[resultArray.Length];

                    for (var i = 0; i < typedResult.Length; i++)
                    {
                        var current = resultArray[i];

                        if (current.IsNull)
                        {
                            typedResult[i] = default;
                        }
                        else
                        {
                            typedResult[i] = SerializerProxy.Deserialize<TResult>(current);
                        }
                    }

                    return typedResult;
                default:
                    throw new ArgumentException(nameof(result), "Not sure how to handle this result.");
            }
        }
    }
}