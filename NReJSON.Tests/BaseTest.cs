namespace NReJSON.Tests
{
    public class BaseTest
    {
        private static readonly ISerializerProxy _serializerProxy = new TestJsonSerializer();

        public BaseTest()
        {
            NReJSONSerializer.SerializerProxy = _serializerProxy;
        }
    }
}
