using BlogCore.Application.Features.BlogPost.Queries;

namespace BlogCore.Application.Common.Base
{
    public static class EmptyPagedResult<T> where T : class
    {
        public static PagedResult<T> Create(int pageNumber, int pageSize)
        {
            return new PagedResult<T>
            {
                Items = new List<T>(),
                TotalCount = 0,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = 0
            };
        }
    }
}
