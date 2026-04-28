using BlogCore.Core.Entities;
using MSSQLFlexCrud.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
    }
}
