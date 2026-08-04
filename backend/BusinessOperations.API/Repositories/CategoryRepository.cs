using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Interfaces.Repositories;

namespace BusinessOperations.API.Repositories;

public sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}
