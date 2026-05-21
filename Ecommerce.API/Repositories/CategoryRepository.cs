namespace Ecommerce.API.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        
        public CategoryRepository(AppDbContext context)
       : base(context)
        {
        }
        


    }
}
