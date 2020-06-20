using System;

namespace NReJSON
{
    [Serializable]
    public class NReJSONException : Exception
    {
        public NReJSONException(string message) : base(message) { }

        public NReJSONException(string message, Exception inner) : base(message, inner) { }
    }
}