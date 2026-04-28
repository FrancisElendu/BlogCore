using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.Infrastructure.Repositories
{
    public class CategoryRepository : SpecificationSqlRepository<Category>, ICategoryRepository
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<Category> _dbSet;

        public CategoryRepository(BlogDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Category>();
        }

        //// Category-specific implementations
        public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.SubCategories)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default)
        {
            var allCategories = await _dbSet
                .AsNoTracking()
                .Include(c => c.SubCategories)
                .ToListAsync(cancellationToken);

            return allCategories.Where(c => c.ParentCategoryId == null).ToList();
        }

        public async Task<IReadOnlyList<Category>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .Include(c => c.SubCategories)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetCategoryPostCountAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbSet
                .AsNoTracking()
                .Include(c => c.BlogPosts)
                .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);

            return category?.BlogPosts?.Count(p => p.Status == Core.Enums.PostStatus.Published) ?? 0;
        }
    }
}
