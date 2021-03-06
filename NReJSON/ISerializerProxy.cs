using StackExchange.Redis;

namespace NReJSON
{
    /// <summary>
    /// Interface used to define a proxy to a JSON serializer.
    /// </summary>
    public interface ISerializerProxy
    {
        /// <summary>
        /// Proxy method for accessing the deserialization functionality of your chosen JSON deserializer.
        /// </summary>
        /// <param name="serializedValue">The JSON formatted value that is returned by Redis.</param>
        /// <typeparam name="TResult">The desired result type.</typeparam>
        /// <returns></returns>
        TResult Deserialize<TResult>(RedisResult serializedValue);

        /// <summary>
        /// Proxy method for accessing the serialization functionality of your chosne JSON serializer.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <typeparam name="TObjectType">Type of the object to serialize.</typeparam>
        /// <returns>Serialization result as a string.</returns>
        string Serialize<TObjectType>(TObjectType obj);
    }
}