using FluentValidation.Results;

namespace BlogCore.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new List<string>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures.Select(x => x.ErrorMessage).ToList();
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new List<string>();
        }

        public ValidationException(string message, List<string> errors)
            : base(message)
        {
            Errors = errors;
        }
    }
}
