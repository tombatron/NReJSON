using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using static NReJSON.NReJSONSerializer;

namespace NReJSON
{
    /// <summary>
    /// This class defines the extension methods for StackExchange.Redis that allow
    /// for the interaction with the RedisJson Redis module.
    /// </summary>
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
        /// <param name="key">Key where JSON object is stored.</param>
        /// <param name="path">Defaults to root if not provided.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Integer, specifically the number of paths deleted (0 or more).</returns>
        public static int JsonDelete(this IDatabase db, RedisKey key, string path = ".",
            CommandFlags commandFlags = CommandFlags.None) =>
            (int) db.Execute(JsonCommands.DEL, CombineArguments(key, path), flags: commandFlags);

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
        /// <param name="key">Key where JSON object is stored.</param>
        /// <param name="paths">The path(s) of the JSON properties that you want to return. By default, the entire JSON object will be returned.</param>
        /// <returns></returns>
        public static RedisResult JsonGet(this IDatabase db, RedisKey key, params string[] paths) =>
            db.JsonGet(key, noEscape: true, paths: paths, commandFlags: CommandFlags.None);

        /// <summary>
        /// `JSON.GET`
        /// 
        /// Return the value at `path` as a deserialized value.
        /// 
        /// `NOESCAPE` is `true` by default.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">Key where JSON object is stored.</param>
        /// <param name="paths">The path(s) of the JSON properties that you want to return. By default, the entire JSON object will be returned.</param>
        /// <typeparam name="TResult">The type to deserialize the value as.</typeparam>
        /// <returns></returns>
        public static TResult JsonGet<TResult>(this IDatabase db, RedisKey key, params string[] paths) =>
            db.JsonGet<TResult>(key, noEscape: true, paths: paths, commandFlags: CommandFlags.None);

        /// <summary>
        /// `JSON.GET`
        /// 
        /// Return the value at `path` in JSON serialized form.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">Key where JSON object is stored.</param>
        /// <param name="noEscape">This option will disable the sending of \uXXXX escapes for non-ascii characters. This option should be used for efficiency if you deal mainly with such text.</param>
        /// <param name="indent">Sets the indentation string for nested levels</param>
        /// <param name="newline">Sets the string that's printed at the end of each line</param>
        /// <param name="space">Sets the string that's put between a key and a value</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <param name="paths">The path(s) of the JSON properties that you want to return. By default, the entire JSON object will be returned.</param>
        /// <returns></returns>
        public static RedisResult JsonGet(this IDatabase db, RedisKey key, bool noEscape = false,
            string indent = default, string newline = default, string space = default,
            CommandFlags commandFlags = CommandFlags.None, params string[] paths)
        {
            var args = new List<object> {key};

            if (noEscape)
            {
                args.Add("NOESCAPE");
            }

            if (indent != default)
            {
                args.Add("INDENT");
                args.Add(indent);
            }

            if (newline != default)
            {
                args.Add("NEWLINE");
                args.Add(newline);
            }

            if (space != default)
            {
                args.Add("SPACE");
                args.Add(space);
            }

            foreach (var path in PathsOrDefault(paths, RootPathStringArray))
            {
                args.Add(path);
            }

            return db.Execute(JsonCommands.GET, args, flags: commandFlags);
        }

        /// <summary>
        /// `JSON.GET`
        /// 
        /// Return the value at `path` as a deserialized value.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">Key where JSON object is stored.</param>
        /// <param name="noEscape">This option will disable the sending of \uXXXX escapes for non-ascii characters. This option should be used for efficiency if you deal mainly with such text.</param>
        /// <param name="indent">Sets the indentation string for nested levels</param>
        /// <param name="newline">Sets the string that's printed at the end of each line</param>
        /// <param name="space">Sets the string that's put between a key and a value</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <param name="paths">The path(s) of the JSON properties that you want to return. By default, the entire JSON object will be returned.</param>
        /// <typeparam name="TResult">The type to deserialize the value as.</typeparam>
        /// <returns></returns>
        public static TResult JsonGet<TResult>(this IDatabase db, RedisKey key, bool noEscape = false,
            string indent = default, string newline = default, string space = default,
            CommandFlags commandFlags = CommandFlags.None, params string[] paths) =>
            SerializerProxy.Deserialize<TResult>(db.JsonGet(key, noEscape, indent, newline, space, commandFlags,
                paths));

        /// <summary>
        /// `JSON.MGET`
        /// 
        /// Returns the values at `path` from multiple `key`s. Non-existing keys and non-existing paths are reported as null.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonmget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys">Keys where JSON objects are stored.</param>
        /// <param name="path">The path of the JSON property that you want to return for each key. This is "root" by default.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Array of Bulk Strings, specifically the JSON serialization of the value at each key's path.</returns>
        public static RedisResult[] JsonMultiGet(this IDatabase db, string[] keys, string path = ".",
            CommandFlags commandFlags = CommandFlags.None) =>
            db.JsonMultiGet(keys.Select(k => (RedisKey) k).ToArray(), path, commandFlags: commandFlags);

        /// <summary>
        /// `JSON.MGET`
        /// 
        /// Returns the values at `path` from multiple `key`s. Non-existing keys and non-existing paths are reported as null.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonmget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys">Keys where JSON objects are stored.</param>
        /// <param name="path">The path of the JSON property that you want to return for each key. This is "root" by default.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Array of Bulk Strings, specifically the JSON serialization of the value at each key's path.</returns>
        public static RedisResult[] JsonMultiGet(this IDatabase db, RedisKey[] keys, string path = ".",
            CommandFlags commandFlags = CommandFlags.None) =>
            (RedisResult[]) db.Execute(JsonCommands.MGET, CombineArguments(keys, path), flags: commandFlags);

        /// <summary>
        /// `JSON.MGET`
        /// 
        /// Returns an IEnumerable of the specified result type. Non-existing keys and non-existent paths are returnd as type default.
        ///  
        /// https://oss.redislabs.com/rejson/commands/#jsonmget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys">Keys where JSON objects are stored.</param>
        /// <param name="path">The path of the JSON property that you want to return for each key. This is "root" by default.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <typeparam name="TResult">The type to deserialize the value as.</typeparam>
        /// <returns>IEnumerable of TResult, non-existent paths/keys are returned as default(TResult).</returns>
        public static IEnumerable<TResult> JsonMultiGet<TResult>(this IDatabase db, RedisKey[] keys, string path = ".",
            CommandFlags commandFlags = CommandFlags.None)
        {
            var serializedResults = db.JsonMultiGet(keys, path, commandFlags: commandFlags);

            foreach (var serializedResult in serializedResults)
            {
                if (serializedResult.IsNull)
                {
                    yield return default(TResult);
                }
                else
                {
                    yield return SerializerProxy.Deserialize<TResult>(serializedResult);
                }
            }
        }

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
        /// <param name="key">Key where JSON object is to be stored.</param>
        /// <param name="json">The JSON object which you want to persist.</param>
        /// <param name="path">The path which you want to persist the JSON object. For new objects this must be root.</param>
        /// <param name="setOption">By default the object will be overwritten, but you can specify that the object be set only if it doesn't already exist or to set only IF it exists.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>An `OperationResult` indicating success or failure.</returns>
        public static OperationResult JsonSet(this IDatabase db, RedisKey key, string json, string path = ".",
            SetOption setOption = SetOption.Default, CommandFlags commandFlags = CommandFlags.None)
        {
            var result = db.Execute(JsonCommands.SET,
                    CombineArguments(key, path, json, GetSetOptionString(setOption)),
                    flags: commandFlags)
                .ToString();

            return new OperationResult(result == "OK", result);
        }

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
        /// <param name="key">Key where JSON object is to be stored.</param>
        /// <param name="obj">The object to serialize and send.</param>
        /// <param name="path">The path which you want to persist the JSON object. For new objects this must be root.</param>
        /// <param name="setOption">By default the object will be overwritten, but you can specify that the object be set only if it doesn't already exist or to set only IF it exists.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <typeparam name="TObjectType">Type of the object being serialized.</typeparam>
        /// <returns>An `OperationResult` indicating success or failure.</returns>
        public static OperationResult JsonSet<TObjectType>(this IDatabase db, RedisKey key, TObjectType obj,
            string path = ".", SetOption setOption = SetOption.Default, CommandFlags commandFlags = CommandFlags.None) =>
            db.JsonSet(key, SerializerProxy.Serialize(obj), path, setOption, commandFlags: commandFlags);

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
        /// <param name="key">The key of the JSON object you need the type of.</param>
        /// <param name="path">The path of the JSON object you want the type of. This defaults to root.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns></returns>
        public static RedisResult JsonType(this IDatabase db, RedisKey key, string path = ".",
            CommandFlags commandFlags = CommandFlags.None) =>
            db.Execute(JsonCommands.TYPE, CombineArguments(key, path), flags: commandFlags);

        /// <summary>
        /// `JSON.NUMINCRBY`
        /// 
        /// Increments the number value stored at `path` by `number`.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonnumincrby
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The key of the JSON object which contains the number value you want to increment.</param>
        /// <param name="path">The path of the JSON value you want to increment.</param>
        /// <param name="number">The value you want to increment by.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        public static RedisResult JsonIncrementNumber(this IDatabase db, RedisKey key, string path, double number,
            CommandFlags commandFlags = CommandFlags.None) =>
            db.Execute(JsonCommands.NUMINCRBY, CombineArguments(key, path, number), flags: commandFlags);

        /// <summary>
        /// `JSON.NUMMULTBY`
        /// 
        /// Multiplies the number value stored at `path` by `number`.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonnummultby
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">They key of the JSON object which contains the number value you want to multiply.</param>
        /// <param name="path">The path of the JSON value you want to multiply.</param>
        /// <param name="number">The value you want to multiply by.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        public static RedisResult JsonMultiplyNumber(this IDatabase db, RedisKey key, string path, double number,
            CommandFlags commandFlags = CommandFlags.None) =>
            db.Execute(JsonCommands.NUMMULTBY, CombineArguments(key, path, number), flags: commandFlags);

        /// <summary>
        /// `JSON.STRAPPEND`
        /// 
        /// Append the json-string value(s) the string at path.
        /// path defaults to root if not provided.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonstrappend
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The key of the JSON object you need to append a string value.</param>
        /// <param name="path">The path of the JSON string you want to append do.  This defaults to root.</param>
        /// <param name="jsonString"></param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Length of the new JSON string.</returns>
        public static int JsonAppendJsonString(this IDatabase db, RedisKey key, string path = ".",
            string jsonString = "\"\"", CommandFlags commandFlags = CommandFlags.None) =>
            (int) db.Execute(JsonCommands.STRAPPEND, CombineArguments(key, path, jsonString), flags: commandFlags);

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
        /// <param name="key">The key of the JSON object you need string length information about.</param>
        /// <param name="path">The path of the JSON string you want the length of. This defaults to root.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Integer, specifically the string's length.</returns>
        public static int? JsonStringLength(this IDatabase db, RedisKey key, string path = ".",
            CommandFlags commandFlags = CommandFlags.None)
        {
            var result = db.Execute(JsonCommands.STRLEN, CombineArguments(key, path), flags: commandFlags);

            if (result.IsNull)
            {
                return null;
            }
            else
            {
                return (int) result;
            }
        }

        /// <summary>
        /// `JSON.ARRAPPEND`
        /// 
        /// Append the `json` value(s) into the array at `path` after the last element in it.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrappend
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The key of the JSON object that contains the array you want to append to.</param>
        /// <param name="path">The path to the JSON array you want to append to.</param>
        /// <param name="json">The JSON values that you want to append.</param>
        /// <returns>Integer, specifically the array's new size.</returns>
        public static int JsonArrayAppend(this IDatabase db, RedisKey key, string path, params object[] json) =>
            JsonArrayAppend(db, key, path, CommandFlags.None, json);

        /// <summary>
        /// `JSON.ARRAPPEND`
        /// 
        /// Append the `json` value(s) into the array at `path` after the last element in it.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrappend
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The key of the JSON object that contains the array you want to append to.</param>
        /// <param name="path">The path to the JSON array you want to append to.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <param name="json">The JSON values that you want to append.</param>
        /// <returns>Integer, specifically the array's new size.</returns>
        public static int JsonArrayAppend(this IDatabase db, RedisKey key, string path,
            CommandFlags commandFlags = CommandFlags.None, params object[] json) =>
            (int) db.Execute(JsonCommands.ARRAPPEND, CombineArguments(key, path, json), flags: commandFlags);

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
        /// <param name="key">The key of the JSON object that contains the array you want to check for a scalar value in.</param>
        /// <param name="path">The path to the JSON array that you want to check.</param>
        /// <param name="jsonScalar">The JSON object that you are looking for.</param>
        /// <param name="start">Where to start searching, defaults to 0 (the beginning of the array).</param>
        /// <param name="stop">Where to stop searching, defaults to 0 (the end of the array).</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Integer, specifically the position of the scalar value in the array, or -1 if unfound.</returns>
        public static int JsonArrayIndexOf(this IDatabase db, RedisKey key, string path, object jsonScalar,
            int start = 0, int stop = 0, CommandFlags commandFlags = CommandFlags.None) =>
            (int) db.Execute(JsonCommands.ARRINDEX, CombineArguments(key, path, jsonScalar, start, stop),
                flags: commandFlags);

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
        /// <param name="key">The key of the JSON object that contains the array you want to insert an object into.</param>
        /// <param name="path">The path of the JSON array that you want to insert into.</param>
        /// <param name="index">The index at which you want to insert, 0 prepends and negative values are interpreted as starting from the end.</param>
        /// <param name="json">The object that you want to insert.</param>
        /// <returns>Integer, specifically the array's new size.</returns>
        public static int JsonArrayInsert(this IDatabase db, RedisKey key, string path, int index,
            params object[] json) =>
            JsonArrayInsert(db, key, path, index, CommandFlags.None, json);

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
        /// <param name="key">The key of the JSON object that contains the array you want to insert an object into.</param>
        /// <param name="path">The path of the JSON array that you want to insert into.</param>
        /// <param name="index">The index at which you want to insert, 0 prepends and negative values are interpreted as starting from the end.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <param name="json">The object that you want to insert.</param>
        /// <returns>Integer, specifically the array's new size.</returns>
        public static int JsonArrayInsert(this IDatabase db, RedisKey key, string path, int index,
            CommandFlags commandFlags = CommandFlags.None,
            params object[] json) =>
            (int) db.Execute(JsonCommands.ARRINSERT, CombineArguments(key, path, index, json), flags: commandFlags);

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
        /// <param name="key">The key of the JSON object that contains the array you want the length of.</param>
        /// <param name="path">The path to the JSON array that you want the length of.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Integer, specifically the array's length.</returns>
        public static int? JsonArrayLength(this IDatabase db, RedisKey key, string path = ".",
            CommandFlags commandFlags = CommandFlags.None)
        {
            var result = db.Execute(JsonCommands.ARRLEN, CombineArguments(key, path), flags: commandFlags);

            if (result.IsNull)
            {
                return null;
            }
            else
            {
                return (int) result;
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
        /// <param name="key">The key of the JSON object that contains the array you want to pop an object off of.</param>
        /// <param name="path">Defaults to root (".") if not provided.</param>
        /// <param name="index">Is the position in the array to start popping from (defaults to -1, meaning the last element).</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Bulk String, specifically the popped JSON value.</returns>
        public static RedisResult JsonArrayPop(this IDatabase db, RedisKey key, string path = ".", int index = -1,
            CommandFlags commandFlags = CommandFlags.None) =>
            db.Execute(JsonCommands.ARRPOP, CombineArguments(key, path, index), flags: commandFlags);

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
        /// <param name="key">The key of the JSON object that contains the array you want to pop an object off of.</param>
        /// <param name="path">Defaults to root (".") if not provided.</param>
        /// <param name="index">Is the position in the array to start popping from (defaults to -1, meaning the last element).</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <typeparam name="TResult">The type to deserialize the value as.</typeparam>
        /// <returns></returns>
        public static TResult JsonArrayPop<TResult>(this IDatabase db, RedisKey key, string path = ".", int index = -1,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var result = db.JsonArrayPop(key, path, index, commandFlags: commandFlags);

            return SerializerProxy.Deserialize<TResult>(result);
        }

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
        /// <param name="key">The key of the JSON object that contains the array you want to trim.</param>
        /// <param name="path">The path of the JSON array that you want to trim.</param>
        /// <param name="start">The inclusive start index.</param>
        /// <param name="stop">The inclusive stop index.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns></returns>
        public static int JsonArrayTrim(this IDatabase db, RedisKey key, string path, int start, int stop,
            CommandFlags commandFlags = CommandFlags.None) =>
            (int) db.Execute(JsonCommands.ARRTRIM, CombineArguments(key, path, start, stop), flags: commandFlags);

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
        /// <param name="key">The key of the JSON object which you want to enumerate keys for.</param>
        /// <param name="path">The path to the JSON object you want the keys for, this defaults to root.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Array, specifically the key names in the object as Bulk Strings.</returns>
        public static RedisResult[] JsonObjectKeys(this IDatabase db, RedisKey key, string path = ".",
            CommandFlags commandFlags = CommandFlags.None) =>
            (RedisResult[]) db.Execute(JsonCommands.OBJKEYS, CombineArguments(key, path), flags: commandFlags);

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
        /// <param name="key">The key of the JSON object which you want the length of.</param>
        /// <param name="path">The path to the JSON object which you want the length of, defaults to root.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Integer, specifically the number of keys in the object.</returns>
        public static int? JsonObjectLength(this IDatabase db, RedisKey key, string path = ".",
            CommandFlags commandFlags = CommandFlags.None)
        {
            var result = db.Execute(JsonCommands.OBJLEN, CombineArguments(key, path), flags: commandFlags);

            if (result.IsNull)
            {
                return null;
            }
            else
            {
                return (int) result;
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
        /// <param name="key">The key of the JSON object that you want to determine the memory usage of.</param>
        /// <param name="path">The path to JSON object you want to check, this defaults to root.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Integer, specifically the size in bytes of the value</returns>
        public static int JsonDebugMemory(this IDatabase db, RedisKey key, string path = ".",
            CommandFlags commandFlags = CommandFlags.None) =>
            (int) db.Execute(JsonCommands.DEBUG, CombineArguments("MEMORY", key.ToString(), path), flags: commandFlags);

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
        /// <param name="key">The key of the JSON object that you want an RESP result for.</param>
        /// <param name="path">Defaults to root if not provided. </param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns>Array, specifically the JSON's RESP form as detailed.</returns>
        public static RedisResult[] JsonGetResp(this IDatabase db, RedisKey key, string path = ".",
            CommandFlags commandFlags = CommandFlags.None) =>
            (RedisResult[]) db.Execute(JsonCommands.RESP, new object[]
            {
                key, path
            }, flags: commandFlags);

        /// <summary>
        /// `JSON.TOGGLE`
        /// 
        /// Toggle the boolean property of a JSON object.
        /// 
        /// Official documentation forthcoming.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The key of the JSON object that contains the property that you'd like to toggle.</param>
        /// <param name="path">The path to the boolean property on JSON object that you'd like to toggle.</param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns></returns>
        public static bool JsonToggle(this IDatabase db, RedisKey key, string path,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var result = db.Execute(JsonCommands.TOGGLE, new object[] {key, path}, flags: commandFlags);

            return bool.Parse(result.ToString());
        }

        /// <summary>
        /// `JSON.CLEAR`
        /// 
        /// Clear/empty arrays and objects (to have zero slots/keys without deleting the array/object) returning the count 
        /// of cleared paths (ignoring non-array and non-objects paths).
        /// 
        /// Official documentation forthcoming.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="commandFlags">Optional command flags.</param>
        /// <returns></returns>
        public static int JsonClear(this IDatabase db, RedisKey key, string path,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var result = db.Execute(JsonCommands.CLEAR, new object[] {key, path}, flags: commandFlags);

            return (int) result;
        }
    }
}