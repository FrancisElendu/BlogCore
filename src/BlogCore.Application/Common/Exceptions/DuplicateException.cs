namespace BlogCore.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a duplicate entity is detected
    /// </summary>
    public class DuplicateException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DuplicateException class
        /// </summary>
        public DuplicateException()
            : base("A duplicate entry was detected.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the DuplicateException class with a specified error message
        /// </summary>
        /// <param name="message">The error message</param>
        public DuplicateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DuplicateException class with entity name and property
        /// </summary>
        /// <param name="entityName">The entity name</param>
        /// <param name="propertyName">The property that caused the duplicate</param>
        /// <param name="propertyValue">The duplicate value</param>
        public DuplicateException(string entityName, string propertyName, string propertyValue)
            : base($"{entityName} with {propertyName} '{propertyValue}' already exists.")
        {
            EntityName = entityName;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        /// <summary>
        /// Initializes a new instance of the DuplicateException class with entity name and composite key
        /// </summary>
        /// <param name="entityName">The entity name</param>
        /// <param name="compositeKey">The composite key values</param>
        public DuplicateException(string entityName, params (string Property, string Value)[] compositeKey)
            : base($"{entityName} with ({string.Join(", ", compositeKey.Select(k => $"{k.Property}='{k.Value}'"))}) already exists.")
        {
            EntityName = entityName;
            CompositeKey = compositeKey.ToDictionary(k => k.Property, k => k.Value);
        }

        /// <summary>
        /// Initializes a new instance of the DuplicateException class with a specified error message
        /// and a reference to the inner exception
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public DuplicateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the entity name that caused the duplicate
        /// </summary>
        public string? EntityName { get; }

        /// <summary>
        /// Gets the property name that caused the duplicate
        /// </summary>
        public string? PropertyName { get; }

        /// <summary>
        /// Gets the duplicate property value
        /// </summary>
        public string? PropertyValue { get; }

        /// <summary>
        /// Gets the composite key that caused the duplicate
        /// </summary>
        public Dictionary<string, string>? CompositeKey { get; }
    }
}
