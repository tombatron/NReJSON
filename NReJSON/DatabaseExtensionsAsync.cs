using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

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
        public static async Task<int> JsonDeleteAsync(this IDatabase db, RedisKey key, string path = ".") =>
            (int)(await db.ExecuteAsync(GetCommandName(CommandType.Json.DEL), CombineArguments(key, path)));

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
        public static Task<RedisResult> JsonGetAsync(this IDatabase db, RedisKey key, params string[] paths) =>
            db.JsonGetAsync(key, true, paths);

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
        public static Task<RedisResult> JsonGetAsync(this IDatabase db, RedisKey key, bool noEscape, params string[] paths) =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.GET), CombineArguments(key, noEscape ? "NOESCAPE" : string.Empty, PathsOrDefault(paths, new[] { "." })));

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
        public static Task<RedisResult[]> JsonMultiGetAsync(this IDatabase db, string[] keys, string path = ".") =>
            db.JsonMultiGetAsync(keys.Select(k => (RedisKey)k).ToArray(), path);

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
        public static async Task<RedisResult[]> JsonMultiGetAsync(this IDatabase db, RedisKey[] keys, string path = ".") =>
            (RedisResult[])(await db.ExecuteAsync(GetCommandName(CommandType.Json.MGET), CombineArguments(keys, path)));

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
        public static Task<RedisResult> JsonSetAsync(this IDatabase db, RedisKey key, string json, string path = ".", SetOption setOption = SetOption.Default) =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.SET), CombineArguments(key, path, json, GetSetOptionString(setOption)));

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
        public static Task<RedisResult> JsonTypeAsync(this IDatabase db, RedisKey key, string path = ".") =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.TYPE), CombineArguments(key, path));

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
        public static Task<RedisResult> JsonIncrementNumberAsync(this IDatabase db, RedisKey key, string path, double number) =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.NUMINCRBY), CombineArguments(key, path, number));

        public static Task JsonMultiplyNumberAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonAppendStringAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonStringLengthAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonArrayAppendAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonArrayIndexOfAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonArrayInsertAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonArrayLengthAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonArrayPopAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonArrayTrimAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonObjectKeysAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonObjectLengthAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonDebugAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonForgetAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonGetRespAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }
    }
}