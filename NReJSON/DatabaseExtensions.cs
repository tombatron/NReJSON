using System.Collections.Generic;
using StackExchange.Redis;
using System;
using System.Linq;

namespace NReJSON
{
    public static partial class DatabaseExtensions
    {
        public static int JsonDelete(this IDatabase db, RedisKey key, string path = "") =>
            (int)db.Execute(GetCommandName(CommandType.Json.DEL), new { key, path });

        public static RedisResult JsonGet(this IDatabase db, RedisKey key, params string[] paths) =>
            db.Execute(GetCommandName(CommandType.Json.GET), new string[] { key }.Concat(paths).ToArray());

        public static RedisResult[] JsonMultiGet(this IDatabase db, RedisKey[] keys, string path = ".") =>
            (RedisResult[])db.Execute(GetCommandName(CommandType.Json.MGET), CombineArguments(keys, path));

        public static RedisResult JsonSet(this IDatabase db, RedisKey key, string json, string path = ".", SetOption setOption = SetOption.Default) =>
            db.Execute(GetCommandName(CommandType.Json.SET), new string[] { key, path, json, });

        public static void JsonType(this IDatabase db)
        {

        }

        public static void JsonIncrementNumber(this IDatabase db)
        {

        }

        public static void JsonMultiplyNumber(this IDatabase db)
        {

        }

        public static void JsonAppendString(this IDatabase db)
        {

        }

        public static void JsonStringLength(this IDatabase db)
        {

        }

        public static void JsonArrayAppend(this IDatabase db)
        {

        }

        public static void JsonArrayIndexOf(this IDatabase db)
        {

        }

        public static void JsonArrayInsert(this IDatabase db)
        {

        }

        public static void JsonArrayLength(this IDatabase db)
        {

        }

        public static void JsonArrayPop(this IDatabase db)
        {

        }

        public static void JsonArrayTrim(this IDatabase db)
        {

        }

        public static void JsonObjectKeys(this IDatabase db)
        {

        }

        public static void JsonObjectLength(this IDatabase db)
        {

        }

        public static void JsonDebug(this IDatabase db)
        {

        }

        public static void JsonForget(this IDatabase db)
        {

        }

        public static void JsonGetResp(this IDatabase db)
        {

        }

        private static string GetCommandName(CommandType.Json jsonCommandType)
        {
            return $"JSON.{jsonCommandType.ToString()}";
        }

        public static string[] CombineArguments(RedisKey[] keys, params string[] arguments) =>
            keys.Select(k => k.ToString()).Concat(arguments).ToArray();
    }
}