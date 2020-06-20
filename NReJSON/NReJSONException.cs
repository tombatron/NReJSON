using System;

namespace NReJSON
{
    /// <summary>
    /// NReJSONException is used to describe exceptions that occur within the NReJSON
    /// library.null 
    /// </summary>
    [Serializable]
    public class NReJSONException : Exception
    {
        /// <summary>
        /// Initialize an instance of NReJSONException with a simple message.
        /// </summary>
        /// <param name="message">The message that you want populated in the `Message` property of the exception.</param>
        /// <returns></returns>
        public NReJSONException(string message) : base(message) { }

        /// <summary>
        /// Initialize an instance of NReJSONException with a simple message and wrap another exception.
        /// </summary>
        /// <param name="message">The message that you want populated in the `Message` property of the exception.</param>
        /// <param name="inner">The exception that you want to wrap.</param>
        /// <returns></returns>
        public NReJSONException(string message, Exception inner) : base(message, inner) { }
    }
}