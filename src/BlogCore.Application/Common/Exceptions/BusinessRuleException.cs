namespace BlogCore.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a business rule is violated
    /// </summary>
    public class BusinessRuleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the BusinessRuleException class
        /// </summary>
        public BusinessRuleException()
            : base("A business rule violation has occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the BusinessRuleException class with a specified error message
        /// </summary>
        /// <param name="message">The error message</param>
        public BusinessRuleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BusinessRuleException class with a specified error message
        /// and a reference to the inner exception
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public BusinessRuleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BusinessRuleException class with rule name and details
        /// </summary>
        /// <param name="ruleName">The name of the violated business rule</param>
        /// <param name="details">Additional details about the violation</param>
        public BusinessRuleException(string ruleName, string details)
            : base($"Business rule '{ruleName}' violated: {details}")
        {
            RuleName = ruleName;
            Details = details;
        }

        /// <summary>
        /// Gets the name of the violated business rule
        /// </summary>
        public string? RuleName { get; }

        /// <summary>
        /// Gets additional details about the violation
        /// </summary>
        public string? Details { get; }
    }
}
