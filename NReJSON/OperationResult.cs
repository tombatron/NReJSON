namespace NReJSON
{
    /// <summary>
    /// This struct captures the result of an operation that returns either
    /// `OK` or an error message.
    /// 
    /// This struct is implicitly convertable to `bool`.
    /// </summary>
    public readonly struct OperationResult
    {
        /// <summary>
        /// A boolean value indicating whether or not an operation succeeded.
        /// </summary>
        /// <value></value>
        public bool IsSuccess { get; }

        /// <summary>
        /// Is an operation didn't succeed the error message will be found here.
        /// </summary>
        /// <value></value>
        public string ErrorMessage { get; }

        /// <summary>
        /// Construct the OperationResult.
        /// </summary>
        /// <param name="isSuccess">True, if the operation succeeded and false, if the operation didn't succeed.</param>
        /// <param name="errorMessage">If the operation didn't succeed the error message will be found here.</param>
        public OperationResult(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Provides the ability to implicitly convert the `OperationalResult` to a boolean.
        /// </summary>
        /// <param name="operationResult"></param>
        public static implicit operator bool(OperationResult operationResult) =>
            operationResult.IsSuccess;
    }
}