using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using NReJSON.Commands;

namespace NReJSON
{
    public static partial class DatabaseExtensions
    {
        public static async Task<int> JsonDeleteAsync(this IDatabase db, RedisKey key, string path = "")
        {
            var result = await db.ExecuteAsync(new Delete(key, path)).ConfigureAwait(false);

            return (int)result;
        }

        public static async Task<string> JsonGetAsync(this IDatabase db, RedisKey key, params string[] paths)
        {
            var result = await db.ExecuteAsync(new Get(key, paths)).ConfigureAwait(false);

            return (string)result;
        }

        public static Task JsonMultiGetAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonSetAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

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