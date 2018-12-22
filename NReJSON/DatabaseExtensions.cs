using StackExchange.Redis;
using System;
using System.Linq;

namespace NReJSON
{
    public static partial class DatabaseExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int JsonDelete(this IDatabase db, RedisKey key, string path = "") =>
            (int)db.Execute(GetCommandName(CommandType.Json.DEL), new { key, path });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static RedisResult JsonGet(this IDatabase db, RedisKey key, params string[] paths) =>
            db.Execute(GetCommandName(CommandType.Json.GET), new string[] { key }.Concat(paths).ToArray());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static RedisResult[] JsonMultiGet(this IDatabase db, RedisKey[] keys, string path = ".") =>
            (RedisResult[])db.Execute(GetCommandName(CommandType.Json.MGET), CombineArguments(keys, path));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <param name="setOption"></param>
        /// <returns></returns>
        public static RedisResult JsonSet(this IDatabase db, RedisKey key, string json, string path = ".", SetOption setOption = SetOption.Default) =>
            db.Execute(GetCommandName(CommandType.Json.SET), new string[] { key, path, json, });

        /// <summary>
        /// `JSON.TYPE`
        /// 
        /// Report the type of JSON value at `path`.
        ///
        /// `path` defaults to root if not provided.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsontype
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static RedisResult JsonType(this IDatabase db, RedisKey key, string path = ".") =>
            db.Execute(GetCommandName(CommandType.Json.TYPE), CombineArguments(key, path));

        /// <summary>
        /// `JSON.NUMINCRBY`
        /// 
        /// Increments the number value stored at `path` by `number`.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonnumincrby
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="number"></param>
        public static RedisResult JsonIncrementNumber(this IDatabase db, RedisKey key, string path, double number) =>
            db.Execute(GetCommandName(CommandType.Json.NUMINCRBY), CombineArguments(key, path, number.ToString()));

        /// <summary>
        /// `JSON.NUMMULTBY`
        /// 
        /// Multiplies the number value stored at `path` by `number`.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonnummultby
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="number"></param>
        public static RedisResult JsonMultiplyNumber(this IDatabase db, RedisKey key, string path, double number) =>
            db.Execute(GetCommandName(CommandType.Json.NUMMULTBY), CombineArguments(key, path, number.ToString()));

        /// <summary>
        /// `JSON.STRAPPEND`
        /// 
        /// Append the json-string value(s) the string at path.
        ///
        /// path defaults to root if not provided.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonstrappend
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="jsonString"></param>
        /// <returns>Length of the new JSON object.</returns>
        public static int JsonAppendJsonString(this IDatabase db, RedisKey key, string path = ".", string jsonString = "{}") =>
            //(int)db.Execute(GetCommandName(CommandType.Json.STRAPPEND), CombineArguments(key, path, jsonString));
            throw new NotImplementedException("This doesn't work, not sure what I'm doing wrong here.");

        /// <summary>
        /// `JSON.STRLEN`
        /// 
        /// Report the length of the JSON String at `path` in `key`.
        ///
        /// `path` defaults to root if not provided. If the `key` or `path` do not exist, null is returned.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonstrlen
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns>Integer, specifically the string's length.</returns>
        public static int JsonStringLength(this IDatabase db, RedisKey key, string path) =>
            (int)db.Execute(GetCommandName(CommandType.Json.STRLEN), CombineArguments(key, path));

        /// <summary>
        /// `JSON.ARRAPPEND`
        /// 
        /// Append the `json` value(s) into the array at `path` after the last element in it.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrappend
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="json"></param>
        /// <returns>Integer, specifically the array's new size.</returns>
        public static int JsonArrayAppend(this IDatabase db, RedisKey key, string path, params string[] json) =>
            (int)db.Execute(GetCommandName(CommandType.Json.ARRAPPEND), CombineArguments(key, path, json));

        /// <summary>
        /// `JSON.ARRINDEX`
        /// 
        /// Search for the first occurrence of a scalar JSON value in an array.
        ///
        /// The optional inclusive `start`(default 0) and exclusive `stop`(default 0, meaning that the last element is included) specify a slice of the array to search.
        ///
        /// Note: out of range errors are treated by rounding the index to the array's start and end. An inverse index range (e.g. from 1 to 0) will return unfound.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrindex
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="jsonScalar"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns>Integer, specifically the position of the scalar value in the array, or -1 if unfound.</returns>
        public static int JsonArrayIndexOf(this IDatabase db, RedisKey key, string path, string jsonScalar, int start = 0, int stop = 0) =>
            (int)db.Execute(GetCommandName(CommandType.Json.ARRINDEX), CombineArguments(key, path, jsonScalar, start, stop));

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

        private static string GetCommandName(CommandType.Json jsonCommandType) =>
            $"JSON.{jsonCommandType.ToString()}";

        private static string[] CombineArguments(RedisKey key, params string[] arguments) =>
            new[] { key.ToString() }.Concat(arguments).ToArray();

        private static string[] CombineArguments(RedisKey[] keys, params string[] arguments) =>
            keys.Select(k => k.ToString()).Concat(arguments).ToArray();

        private static string[] CombineArguments(RedisKey key, string path, params object[] arguments) =>
            new[] { key.ToString(), path }.Concat(arguments.Select(a => a.ToString())).ToArray();

        private static string[] CombineArguments(RedisKey key, string argument) =>
            new[] { key.ToString(), argument };
    }
}