using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using static NReJSON.NReJSONSerializer;

namespace NReJSON
{
    /// <summary>
    /// Encapsulates the result of a "pathed" operation such as JsonGet(Async) which can return multiple results based
    /// on whatever path was provided. 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class PathedResult<TResult> : IEnumerable<TResult>
    {
        /// <summary>
        /// The original unparsed result that came back from Redis.
        /// </summary>
        public RedisResult InnerResult { get; }

        private PathedResult(RedisResult redisResult) =>
            InnerResult = redisResult;

        internal static PathedResult<TResult> Create(RedisResult redisResult) =>
            new PathedResult<TResult>(redisResult);

        /// <summary>
        /// Implicit conversion to a single instance of `TResult` for convenience. 
        /// </summary>
        /// <param name="pathedResult"></param>
        /// <returns></returns>
        public static implicit operator TResult(PathedResult<TResult> pathedResult) =>
            pathedResult.Single();

        /// <summary>
        /// Returns the enumerator that will attempt to parse the RedisResult into multiple results.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TResult> GetEnumerator() => ParseRedisResult().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ParseRedisResult().GetEnumerator();

        private IEnumerable<TResult> ParseRedisResult()
        {
            if (InnerResult.IsNull)
            {
                return Enumerable.Empty<TResult>();
            }

            if (IsJsonArray(InnerResult.ToString()))
            {
                return SerializerProxy.Deserialize<IEnumerable<TResult>>(InnerResult);
            }
            else
            {
                return EnumerableFrom(SerializerProxy.Deserialize<TResult>(InnerResult));
            }
        }

        private static bool IsJsonArray(string redisStringResult) => 
            redisStringResult.StartsWith("[") && redisStringResult.EndsWith("]");

        private static IEnumerable<TResult> EnumerableFrom(TResult result)
        {
            yield return result;
        }
    }
}