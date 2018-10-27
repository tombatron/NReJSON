using System;
using System.Linq;
using StackExchange.Redis;

namespace NReJSON
{
    public static partial class DatabaseExtensions
    {
        public static int JsonDelete(this IDatabase db, RedisKey key, string path = "")
        {
            var commandResult = db.Execute(GetCommandName(CommandType.Json.DEL), new { key, path });

            return (int)commandResult;
        }


        public static string JsonGet(this IDatabase db, RedisKey key, params string[] paths)
        {
            var arguments = new string[] { key }.Concat(paths).ToArray();

            var getResult = db.Execute(GetCommandName(CommandType.Json.GET), arguments);

            return getResult.ToString();
        }


        public static void JsonMultiGet(this IDatabase db)
        {

        }

        public static string JsonSet(this IDatabase db, RedisKey key, string json, string path = ".", SetOption setOption = SetOption.Default)
        {
            var setResult = db.Execute(GetCommandName(CommandType.Json.SET), new string[] { key, path, json, });

            return setResult.ToString();
        }


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
            return $"{nameof(CommandType.Json).ToUpperInvariant()}.{jsonCommandType.ToString()}";
        }
    }
}