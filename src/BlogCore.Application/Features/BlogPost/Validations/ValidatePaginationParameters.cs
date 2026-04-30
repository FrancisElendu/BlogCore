using BlogCore.Application.Common.Exceptions;

namespace BlogCore.Application.Features.BlogPost.Validations
{
    public static class ValidatePaginationParameters
    {
        public static void PaginationParameters(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                throw new BusinessRuleException("PageNumber", "Page number must be at least 1.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                throw new BusinessRuleException("PageSize", "Page size must be between 1 and 100.");
            }
        }
    }
}
