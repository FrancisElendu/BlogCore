using MayFlo.Specification.Builder;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.Infrastructure.Repositories
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // Apply filtering
            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            // Apply includes (strongly typed)
            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            query = specification.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));

            // Apply sorting
            IOrderedQueryable<T>? orderedQuery = null;

            if (specification.OrderBys.Any())
            {
                orderedQuery = query.OrderBy(specification.OrderBys[0]);
                for (int i = 1; i < specification.OrderBys.Count; i++)
                {
                    orderedQuery = orderedQuery.ThenBy(specification.OrderBys[i]);
                }
            }
            else if (specification.OrderByDescendings.Any())
            {
                orderedQuery = query.OrderByDescending(specification.OrderByDescendings[0]);
                for (int i = 1; i < specification.OrderByDescendings.Count; i++)
                {
                    orderedQuery = orderedQuery.ThenByDescending(specification.OrderByDescendings[i]);
                }
            }

            if (orderedQuery != null)
                query = orderedQuery;

            // Apply paging
            if (specification.IsPagingEnabled)
                query = query.Skip(specification.Skip).Take(specification.Take);

            return query;
        }
    }
}
