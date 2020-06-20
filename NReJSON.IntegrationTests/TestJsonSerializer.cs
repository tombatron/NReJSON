using StackExchange.Redis;
using System.Text.Json;

namespace NReJSON.IntegrationTests
{
    public sealed class TestJsonSerializer : ISerializerProxy
    {
        public TResult Deserialize<TResult>(RedisResult serializedValue) =>
            JsonSerializer.Deserialize<TResult>(serializedValue.ToString());
    }
}