namespace BlogCore.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a user attempts to access a resource they don't have permission for
    /// </summary>
    public class ForbiddenAccessException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ForbiddenAccessException class
        /// </summary>
        public ForbiddenAccessException()
            : base("You do not have permission to access this resource.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the ForbiddenAccessException class with a specified error message
        /// </summary>
        /// <param name="message">The error message</param>
        public ForbiddenAccessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ForbiddenAccessException class with user and resource info
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="resourceType">The resource type</param>
        /// <param name="resourceId">The resource ID</param>
        public ForbiddenAccessException(Guid userId, string resourceType, Guid resourceId)
            : base($"User '{userId}' does not have permission to access {resourceType} with ID '{resourceId}'.")
        {
            UserId = userId;
            ResourceType = resourceType;
            ResourceId = resourceId;
        }

        /// <summary>
        /// Initializes a new instance of the ForbiddenAccessException class with required permission
        /// </summary>
        /// <param name="requiredPermission">The required permission</param>
        public ForbiddenAccessException(string requiredPermission, string resourceType)
            : base($"You need '{requiredPermission}' permission to access this {resourceType}.")
        {
            RequiredPermission = requiredPermission;
            ResourceType = resourceType;
        }

        /// <summary>
        /// Initializes a new instance of the ForbiddenAccessException class with a specified error message
        /// and a reference to the inner exception
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public ForbiddenAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the user ID that attempted the forbidden access
        /// </summary>
        public Guid? UserId { get; }

        /// <summary>
        /// Gets the resource type that access was attempted for
        /// </summary>
        public string? ResourceType { get; }

        /// <summary>
        /// Gets the resource ID that access was attempted for
        /// </summary>
        public Guid? ResourceId { get; }

        /// <summary>
        /// Gets the required permission for accessing the resource
        /// </summary>
        public string? RequiredPermission { get; }
    }
}
