using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NReJSON
{
    public static partial class DatabaseExtensions
    {
        /// <summary>
        /// `JSON.DEL`
        /// 
        /// Delete a value.
        ///
        /// Non-existing keys and paths are ignored. Deleting an object's root is equivalent to deleting the key from Redis.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsondel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path">Defaults to root if not provided.</param>
        /// <returns>Integer, specifically the number of paths deleted (0 or 1).</returns>
        public static int JsonDelete(this IDatabase db, RedisKey key, string path = ".") =>
            (int)db.Execute(GetCommandName(CommandType.Json.DEL), CombineArguments(key, path));

        /// <summary>
        /// `JSON.GET`
        /// 
        /// Return the value at `path` in JSON serialized form.
        /// 
        /// `NOESCAPE` is `true` by default.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static RedisResult JsonGet(this IDatabase db, RedisKey key, params string[] paths) =>
            db.JsonGet(key, true, paths);

        /// <summary>
        /// `JSON.GET`
        /// 
        /// Return the value at `path` in JSON serialized form.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="noEscape">This option will disable the sending of \uXXXX escapes for non-ascii characters. This option should be used for efficiency if you deal mainly with such text.</param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static RedisResult JsonGet(this IDatabase db, RedisKey key, bool noEscape, params string[] paths) =>
            db.Execute(GetCommandName(CommandType.Json.GET), CombineArguments(key, noEscape ? "NOESCAPE" : string.Empty, PathsOrDefault(paths, new[] { "." })));

        /// <summary>
        /// `JSON.MGET`
        /// 
        /// Returns the values at `path` from multiple `key`s. Non-existing keys and non-existing paths are reported as null.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonmget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys"></param>
        /// <param name="path"></param>
        /// <returns>Array of Bulk Strings, specifically the JSON serialization of the value at each key's path.</returns>
        public static RedisResult[] JsonMultiGet(this IDatabase db, string[] keys, string path = ".") =>
            db.JsonMultiGet(keys.Select(k => (RedisKey)k).ToArray(), path);

        /// <summary>
        /// `JSON.MGET`
        /// 
        /// Returns the values at `path` from multiple `key`s. Non-existing keys and non-existing paths are reported as null.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonmget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys"></param>
        /// <param name="path"></param>
        /// <returns>Array of Bulk Strings, specifically the JSON serialization of the value at each key's path.</returns>
        public static RedisResult[] JsonMultiGet(this IDatabase db, RedisKey[] keys, string path = ".") =>
            (RedisResult[])db.Execute(GetCommandName(CommandType.Json.MGET), CombineArguments(keys, path));

        /// <summary>
        /// `JSON.SET`
        /// 
        /// Sets the JSON value at path in key
        ///
        /// For new Redis keys the path must be the root. 
        /// 
        /// For existing keys, when the entire path exists, the value that it contains is replaced with the json value.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonset
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <param name="setOption"></param>
        /// <returns></returns>
        public static RedisResult JsonSet(this IDatabase db, RedisKey key, string json, string path = ".", SetOption setOption = SetOption.Default) =>
            db.Execute(GetCommandName(CommandType.Json.SET), CombineArguments(key, path, json, GetSetOptionString(setOption)));

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
            db.Execute(GetCommandName(CommandType.Json.NUMINCRBY), CombineArguments(key, path, number));

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
            db.Execute(GetCommandName(CommandType.Json.NUMMULTBY), CombineArguments(key, path, number));

        /// <summary>
        /// [Not implemented yet]
        /// 
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

        /// <summary>
        /// `JSON.ARRINSERT`
        /// 
        /// Insert the `json` value(s) into the array at `path` before the `index` (shifts to the right).
        ///
        /// The index must be in the array's range. Inserting at `index` 0 prepends to the array. Negative index values are interpreted as starting from the end.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrinsert
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="index"></param>
        /// <param name="json"></param>
        /// <returns>Integer, specifically the array's new size.</returns>
        public static int JsonArrayInsert(this IDatabase db, RedisKey key, string path, int index, params string[] json) =>
            (int)db.Execute(GetCommandName(CommandType.Json.ARRINSERT), CombineArguments(key, path, index, json));

        /// <summary>
        /// `JSON.ARRLEN`
        /// 
        /// Report the length of the JSON Array at `path` in `key`.
        /// 
        /// `path` defaults to root if not provided. If the `key` or `path` do not exist, null is returned.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrlen
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns>Integer, specifically the array's length.</returns>
        public static int? JsonArrayLength(this IDatabase db, RedisKey key, string path = ".")
        {
            var result = db.Execute(GetCommandName(CommandType.Json.ARRLEN), CombineArguments(key, path));

            if (result.IsNull)
            {
                return null;
            }
            else
            {
                return (int)result;
            }
        }

        /// <summary>
        /// `JSON.ARRPOP`
        /// 
        /// Remove and return element from the index in the array.
        ///
        /// Out of range indices are rounded to their respective array ends.Popping an empty array yields null.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrpop
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path">Defaults to root (".") if not provided.</param>
        /// <param name="index">Is the position in the array to start popping from (defaults to -1, meaning the last element).</param>
        /// <returns>Bulk String, specifically the popped JSON value.</returns>
        public static RedisResult JsonArrayPop(this IDatabase db, RedisKey key, string path = ".", int index = -1) =>
            db.Execute(GetCommandName(CommandType.Json.ARRPOP), CombineArguments(key, path, index));

        /// <summary>
        /// `JSON.ARRTRIM`
        /// 
        /// Trim an array so that it contains only the specified inclusive range of elements.
        /// 
        /// This command is extremely forgiving and using it with out of range indexes will not produce an error. 
        /// 
        /// If start is larger than the array's size or start > stop, the result will be an empty array. 
        /// 
        /// If start is &lt; 0 then it will be treated as 0. 
        /// 
        /// If stop is larger than the end of the array, it will be treated like the last element in it.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrtrim
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns>New length of affected array.</returns>
        public static int JsonArrayTrim(this IDatabase db, RedisKey key, string path, int start, int stop) =>
            (int)db.Execute(GetCommandName(CommandType.Json.ARRTRIM), CombineArguments(key, path, start, stop));

        /// <summary>
        /// `JSON.OBJKEYS`
        /// 
        /// Return the keys in the object that's referenced by `path`.
        ///
        /// `path` defaults to root if not provided.If the object is empty, or either `key` or `path` do not exist, then null is returned.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonobjkeys
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns>Array, specifically the key names in the object as Bulk Strings.</returns>
        public static RedisResult[] JsonObjectKeys(this IDatabase db, RedisKey key, string path = ".") =>
            (RedisResult[])db.Execute(GetCommandName(CommandType.Json.OBJKEYS), CombineArguments(key, path));

        /// <summary>
        /// `JSON.OBJLEN`
        /// 
        /// Report the number of keys in the JSON Object at `path` in `key`.
        ///
        /// `path` defaults to root if not provided. If the `key` or `path` do not exist, null is returned.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonobjlen
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns>Integer, specifically the number of keys in the object.</returns>
        public static int? JsonObjectLength(this IDatabase db, RedisKey key, string path = ".")
        {
            var result = db.Execute(GetCommandName(CommandType.Json.OBJLEN), CombineArguments(key, path));

            if (result.IsNull)
            {
                return null;
            }
            else
            {
                return (int)result;
            }
        }

        /// <summary>
        /// `JSON.DEBUG MEMORY`
        /// 
        /// Report the memory usage in bytes of a value. `path` defaults to root if not provided.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsondebug
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns>Integer, specifically the size in bytes of the value</returns>
        public static int JsonDebugMemory(this IDatabase db, RedisKey key, string path = ".") =>
            (int)db.Execute(GetCommandName(CommandType.Json.DEBUG), CombineArguments("MEMORY", key.ToString(), path));

        /// <summary>
        /// `JSON.RESP`
        /// 
        /// This command uses the following mapping from JSON to RESP: 
        /// 
        ///     - JSON Null is mapped to the RESP Null Bulk String 
        ///     
        ///     - JSON `false` and `true` values are mapped to the respective RESP Simple Strings 
        ///     
        ///     - JSON Numbers are mapped to RESP Integers or RESP Bulk Strings, depending on type 
        ///     
        ///     - JSON Strings are mapped to RESP Bulk Strings 
        ///     
        ///     - JSON Arrays are represented as RESP Arrays in which the first element is the simple string `[` followed by the array's elements 
        ///     
        ///     - JSON Objects are represented as RESP Arrays in which the first element is the simple string `{`. Each successive entry represents a key-value pair as a two-entries array of bulk strings.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonresp
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path">Defaults to root if not provided. </param>
        /// <returns>Array, specifically the JSON's RESP form as detailed.</returns>
        public static RedisResult[] JsonGetResp(this IDatabase db, RedisKey key, string path = ".") =>
            (RedisResult[])db.Execute(GetCommandName(CommandType.Json.RESP), key, path);

        private static string GetCommandName(CommandType.Json jsonCommandType) =>
            $"JSON.{jsonCommandType.ToString()}";

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
    }
}