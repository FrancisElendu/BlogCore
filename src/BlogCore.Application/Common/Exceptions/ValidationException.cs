using FluentValidation.Results;

namespace BlogCore.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when validation fails
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ValidationException class
        /// </summary>
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class with validation failures
        /// </summary>
        /// <param name="failures">The validation failures</param>
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class with a specific error
        /// </summary>
        /// <param name="propertyName">The property name that failed validation</param>
        /// <param name="errorMessage">The validation error message</param>
        public ValidationException(string propertyName, string errorMessage)
            : this()
        {
            Errors = new Dictionary<string, string[]>
            {
                { propertyName, new[] { errorMessage } }
            };
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class with multiple errors
        /// </summary>
        /// <param name="errors">Dictionary of property names and their validation errors</param>
        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation failures have occurred.")
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Gets the validation errors grouped by property name
        /// </summary>
        public Dictionary<string, string[]> Errors { get; }
    }
}
