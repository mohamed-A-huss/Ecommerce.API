namespace Ecommerce.API.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        
        public BrandRepository(AppDbContext context)
       : base(context)
        {
        }
        


    }
}
