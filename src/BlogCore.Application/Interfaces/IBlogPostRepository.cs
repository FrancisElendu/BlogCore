using BlogCore.Core.Entities;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Interfaces
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
    }
}
