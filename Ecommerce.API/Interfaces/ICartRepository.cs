namespace Ecommerce.API.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        void DeleteRange(IEnumerable<Cart> carts);
    }
}
