namespace NReJSON
{
    /// <summary>
    /// This static class serves as a container for storing a JSON serializer for use by NReJSON
    /// when returning typed results from the RedisJson command.
    /// </summary>
    public static class NReJSONSerializer
    {
        private static object _locker = new object();
        private static bool _serializerSet = false;

        private static volatile ISerializerProxy _serializerProxy = default;

        /// <summary>
        /// This is where you assign your implementation of the `ISerializerProxy` interface.
        /// 
        /// This property is "write-once" meaning, that once it is set it cannot be reset.
        /// 
        /// This property will throw an `NReJSONException` if you try to access it before it
        /// is assigned.
        /// </summary>
        /// <value>An implementation of the ISerializerProxy that is appropriate for your application.</value>
        public static ISerializerProxy SerializerProxy
        {
            get
            {
                if (_serializerSet)
                {
                    return _serializerProxy;
                }

                lock (_locker)
                {
                    if (_serializerProxy is null)
                    {
                        throw new NReJSONException("Attempted to access the ISerializerProxy before it was set. Consider setting `NReJSONSerializer.SerializerProxy` with an implementation of `ISerializerProxy` before proceeding.");
                    }
                    else
                    {
                        return _serializerProxy;
                    }
                }
            }

            set
            {
                if (_serializerSet)
                {
                    return;
                }

                if (_serializerProxy is null)
                {
                    lock (_locker)
                    {
                        if (_serializerProxy is null)
                        {
                            _serializerProxy = value;
                        }
                    }
                }
            }
        }
    }
}