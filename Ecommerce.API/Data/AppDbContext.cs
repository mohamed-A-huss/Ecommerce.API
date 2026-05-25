using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Ecommerce.API.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Mobiles",
                    Status = true
                },
                new Category
                {
                    Id = 2,
                    Name = "Laptops",
                    Status = true
                }
            );

            modelBuilder.Entity<Brand>().HasData(
                new Brand
                {
                    Id = 1,
                    Name = "Apple",
                    Logo = "apple.png",
                    Status = true
                },
                new Brand
                {
                    Id = 2,
                    Name = "Samsung",
                    Logo = "samsung.png",
                    Status = true
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 15",
                    MainImg = "iphone.jpg",
                    Description = "Apple mobile phone",
                    Price = 50000,
                    Discount = 5,
                    Rate = 4.8,
                    Status = true,
                    Traffic = 100,
                    CategoryId = 1,
                    BrandId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung S24",
                    MainImg = "samsung.jpg",
                    Description = "Samsung flagship phone",
                    Price = 40000,
                    Discount = 10,
                    Rate = 4.7,
                    Status = true,
                    Traffic = 90,
                    CategoryId = 1,
                    BrandId = 2
                }
            );
        }
    }
}
