namespace NReJSON
{
    public static class NReJSONSerializer
    {
        private static object _locker = new object();
        private static bool _serializerSet = false;

        private static volatile ISerializerProxy _serializerProxy = default;

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