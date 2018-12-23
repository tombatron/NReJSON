using StackExchange.Redis;
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
            db.ExecuteAsync(GetCommandName(CommandType.Json.GET), CombineArguments(key, noEscape ? "NOESCAPE" : string.Empty, PathsOrDefault(paths, new[] { "."})));

        public static Task JsonMultiGetAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonSetAsync(this IDatabase db, RedisKey key, string json, string path = ".", SetOption setOption = SetOption.Default) =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.SET), new string[] { key, path, json });

        public static Task JsonTypeAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonIncrementNumberAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

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