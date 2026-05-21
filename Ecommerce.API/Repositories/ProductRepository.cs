namespace Ecommerce.API.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        
        public ProductRepository(AppDbContext context)
       : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetTrendingProductsAsync()
        {
            return await _dbSet
                .Where(p => p.Traffic > 100)
                .ToListAsync();
        }
        
    }
}
