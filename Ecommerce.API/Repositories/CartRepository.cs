namespace Ecommerce.API.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context)
        {
        }
        public void DeleteRange(IEnumerable<Cart> carts)
            => _context.RemoveRange(carts);
    }
}
