namespace NReJSON
{
    /// <summary>
    /// An enumeration that captures the available options for setting values.
    /// </summary>
    public enum SetOption
    {
        /// <summary>
        /// Set the value not matter what.    
        /// </summary>
        Default,

        /// <summary>
        /// Set the value only if it doesn't already exist.    
        /// </summary>        
        SetIfNotExists,

        /// <summary>
        /// Set the value only if it already exists.    
        /// </summary>        
        SetOnlyIfExists
    }
}