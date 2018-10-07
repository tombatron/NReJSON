using System.Threading.Tasks;
using NReJSON.Commands;
using StackExchange.Redis;

namespace NReJSON
{
    public static partial class DatabaseExtensions
    {
        public static int JsonDelete(this IDatabase db, RedisKey key, string path = "") =>
            (int)db.Execute(new Delete(key, path));

        public static string JsonGet(this IDatabase db, RedisKey key, params string[] paths) =>
            (string)db.Execute(new Get(key, paths));

        public static void JsonMultiGet(this IDatabase db)
        {

        }

        public static string JsonSet(this IDatabase db, RedisKey key, string json, SetOption setOption = SetOption.Default) =>
            (string)db.Execute(new Set(key, json, setOption));

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
    }
}