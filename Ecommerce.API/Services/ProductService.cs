

using Ecommerce.API.Repositories;

namespace Ecommerce.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IImageService _imageService;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger, IImageService imageService)
        {
            _productRepository = productRepository;
            _logger = logger;
            _imageService = imageService;
        }

        public async Task<PaginatedProductResponseDto> GetAll(FilterProductDto filter ,int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            string applyFilter = "Applied filters: ";
            var products =  await _productRepository.GetAsync(tracked:false,includes: [p => p.Category, p => p.Brand]);
            if (products.Count() == 0)
            {
                _logger.LogInformation("No products found");
                return new PaginatedProductResponseDto
                {
                    Products = Enumerable.Empty<ProductItemDto>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0
                };
            }

            if (filter.Name is not null)
            {
                products = products.Where(
                    e => e.Name.ToLower().Contains(filter.Name.ToLower()));
                applyFilter += $"Name contains '{filter.Name}' ";
            }

            if (filter.Status is not null)
            {
                products = products.Where(e => e.Status == filter.Status);
                applyFilter += $"Status is '{filter.Status}' ";
            }

            if (filter.MinPrice is not null)
            {
                products = products.Where(e => e.Price >= filter.MinPrice);
                applyFilter += $"MinPrice is '{filter.MinPrice}' ";
            }

            if (filter.MaxPrice is not null)
            {
                products = products.Where(e => e.Price <= filter.MaxPrice);
                applyFilter += $"MaxPrice is '{filter.MaxPrice}' ";
            }

            if (filter.Discount is not null)
            {
                products = products.Where(e => e.Discount == filter.Discount);
                applyFilter += $"Discount is '{filter.Discount}' ";
            }

            if (filter.CategoryId is not null)
            {
                products = products.Where(e => e.CategoryId == filter.CategoryId);
                applyFilter += $"CategoryId is '{filter.CategoryId}' ";
            }

            if (filter.BrandId is not null)
            {
                products = products.Where(e => e.BrandId == filter.BrandId);
                applyFilter += $"BrandId is '{filter.BrandId}' ";
            }
            if (filter.Trend)
            {
                products = products.OrderByDescending(e => e.Traffic);
                applyFilter += "Ordered by trending products ";
            }

            _logger.LogInformation(applyFilter);
            // Pagination
            var totalCount = await products.CountAsync();

            double totalPages = Math.Ceiling(totalCount / (double)pageSize);

            products = products.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var result = await products.ToListAsync(cancellationToken);

            return new PaginatedProductResponseDto
            {
                Products = result.Select(p => new ProductItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    MainImg = p.MainImg,
                    Status = p.Status,
                    BrandName = p.Brand.Name,
                    CategoryName = p.Category.Name
                }),
                CurrentPage = pageNumber,
                TotalPages = (int)totalPages,
                TotalCount = totalCount
            };
        }





        public async Task<Product?> CreateAsync([FromForm] CreateProductDto dto)
        {
            try
            {
                var imageName = await _imageService.SaveImageAsync(dto.MainImg, "products");
                var newProduct = new Product
                {
                    Name = dto.Name,
                    MainImg = imageName,
                    Description = dto.Description,
                    Price = dto.Price,
                    Discount = dto.Discount,
                    Status = dto.Status,
                    CategoryId = dto.CategoryId,
                    BrandId = dto.BrandId
                };

                await _productRepository.CreateAsync(newProduct);
                var result = await _productRepository.CommitAsync();
                if (result <= 0)
                {
                    _logger.LogError("Failed to create product");
                    return null;
                }
                _logger.LogInformation("Product with id {id} was created", newProduct.Id);
                return newProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product =  await _productRepository.GetOneAsync(p => p.Id == id);
            if (product is null)
            {
                _logger.LogWarning("Product with id {id} was not found", id);
                return false;
            }
            await _imageService.DeleteImageAsync(product.MainImg, "products");
             _productRepository.Delete(product);
            await _productRepository.CommitAsync();
            _logger.LogInformation("Product with id {id} was deleted", id);
            return true;
        }

        

        public async Task<ProductItemDto?> GetByIdAsync(int id)
        {

           var product =  await _productRepository.GetOneAsync(p => p.Id == id, includes: [p => p.Brand, p => p.Category]);
            if (product is null)
            {
                _logger.LogWarning("Product with id {id} was not found", id);
                return null;
            }
            product.Traffic += 1;
            _productRepository.Update(product);
            await _productRepository.CommitAsync();
            ProductItemDto productItemDto = new ProductItemDto
            {
                Id = product.Id,
                Name = product.Name,
                MainImg = product.MainImg,
                Description = product.Description,
                Price = product.Price,
                Discount = product.Discount,
                Status = product.Status,
                BrandName = product.Brand.Name,
                CategoryName = product.Category.Name,

            };
            _logger.LogInformation("Product with id {id} was found", id);
            return productItemDto;
        }

        public async Task<Product?> UpdateAsync(int id, [FromForm] UpdateProductDto dto)
        {
            var existingProduct =  await _productRepository.GetOneAsync(p => p.Id == id);
            if (existingProduct is null)
            {
                _logger.LogWarning("Product with id {id} was not found", id);
                return null;
            }
            if (dto.MainImg is not null)
            {
                await _imageService.DeleteImageAsync(
                    existingProduct.MainImg, "products");

                var newImageName =
                    await _imageService.SaveImageAsync(dto.MainImg, "products");

                existingProduct.MainImg = newImageName;
            }
            if (dto.Name is not null)
            {
                existingProduct.Name = dto.Name;

            }
            if (dto.Description is not null)
            {
                existingProduct.Description = dto.Description;
            }
            if (dto.Price is not null)
            {
                existingProduct.Price = (decimal)dto.Price;
            }
            if (dto.Discount is not null)
            {
                existingProduct.Discount = (decimal)dto.Discount;
            }
            if (dto.Status is not null)
            {
                existingProduct.Status = (bool)dto.Status;
            }

            _productRepository.Update(existingProduct);
            await _productRepository.CommitAsync();
            _logger.LogInformation("Product with id {id} was updated successfully", id);

            return existingProduct;
        }

        public async Task<bool> ChangeStatusAsync(int id)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == id);
            if (product is null)
            {
                _logger.LogWarning("Product with id {id} was not found", id);
                return false;
            }
            product.Status = !product.Status;
            await _productRepository.CommitAsync();
            _logger.LogInformation("Product with id {id} was updated", id);
            return true;
        }
    }
}
