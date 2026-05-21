namespace Ecommerce.API.Interfaces
{
    public interface IProductRepository:IRepository<Product>
    {
        Task<IEnumerable<Product>> GetTrendingProductsAsync();
    }
}
