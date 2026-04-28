namespace BlogCore.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a concurrency conflict occurs
    /// </summary>
    public class ConcurrencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ConcurrencyException class
        /// </summary>
        public ConcurrencyException()
            : base("A concurrency conflict occurred. The data has been modified by another user.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConcurrencyException class with a specified error message
        /// </summary>
        /// <param name="message">The error message</param>
        public ConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConcurrencyException class with entity name
        /// </summary>
        /// <param name="entityName">The entity name</param>
        /// <param name="entityId">The entity ID</param>
        public ConcurrencyException(string entityName, Guid entityId)
            : base($"{entityName} with ID '{entityId}' has been modified by another user. Please refresh and try again.")
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        /// <summary>
        /// Initializes a new instance of the ConcurrencyException class with entity name and version info
        /// </summary>
        /// <param name="entityName">The entity name</param>
        /// <param name="entityId">The entity ID</param>
        /// <param name="expectedVersion">The expected version</param>
        /// <param name="actualVersion">The actual version</param>
        public ConcurrencyException(string entityName, Guid entityId, int expectedVersion, int actualVersion)
            : base($"{entityName} with ID '{entityId}' has been modified. Expected version {expectedVersion}, but found version {actualVersion}.")
        {
            EntityName = entityName;
            EntityId = entityId;
            ExpectedVersion = expectedVersion;
            ActualVersion = actualVersion;
        }

        /// <summary>
        /// Initializes a new instance of the ConcurrencyException class with a specified error message
        /// and a reference to the inner exception
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public ConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the entity name that caused the concurrency conflict
        /// </summary>
        public string? EntityName { get; }

        /// <summary>
        /// Gets the entity ID that caused the concurrency conflict
        /// </summary>
        public Guid? EntityId { get; }

        /// <summary>
        /// Gets the expected version number
        /// </summary>
        public int? ExpectedVersion { get; }

        /// <summary>
        /// Gets the actual version number
        /// </summary>
        public int? ActualVersion { get; }
    }
}
