namespace BlogCore.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested entity is not found
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NotFoundException class
        /// </summary>
        public NotFoundException()
            : base("The requested resource was not found.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class with a specified error message
        /// </summary>
        /// <param name="message">The error message</param>
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class with entity name and key
        /// </summary>
        /// <param name="name">The entity name</param>
        /// <param name="key">The entity key</param>
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
            Name = name;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the entity name that was not found
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the key value that was not found
        /// </summary>
        public object? Key { get; }
    }
}
