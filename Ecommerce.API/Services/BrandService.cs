

namespace Ecommerce.API.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly ILogger<BrandService> _logger;
        private readonly IImageService _imageService;
        public BrandService(IBrandRepository brandRepository, ILogger<BrandService> logger, IImageService imageService)
        {
            _brandRepository = brandRepository;
            _logger = logger;
            _imageService = imageService;
        }

        public async Task<PaginatedBrandResponseDto> GetAll(FilterBrandDto filter ,int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            string applyFilter = "Applied filters: ";
            var brands =  await _brandRepository.GetAsync();
            if (brands.Count() == 0)
            {
                _logger.LogInformation("No brands found");
                return new PaginatedBrandResponseDto
                {
                    Brands = Enumerable.Empty<BrandItemDto>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0
                };
            }

            if (filter.Name is not null)
            {
                brands = brands.Where(
                    e => e.Name.ToLower().Contains(filter.Name.ToLower()));
                applyFilter += $"Name contains '{filter.Name}' ";
            }

            if (filter.Status is not null)
            {
                brands = brands.Where(e => e.Status == filter.Status);
                applyFilter += $"Status is '{filter.Status}' ";
            }

            

            _logger.LogInformation(applyFilter);
            // Pagination
            var totalCount = await brands.CountAsync();

            double totalPages = Math.Ceiling(totalCount / (double)pageSize);

            brands = brands.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var result = await brands.ToListAsync(cancellationToken);

            return new PaginatedBrandResponseDto
            {
                Brands = result.Select(p => new BrandItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    Logo = p.Logo

                }),
                CurrentPage = pageNumber,
                TotalPages = (int)totalPages,
                TotalCount = totalCount
            };
        }





        public async Task<Brand?> CreateAsync([FromForm] CreateBrandDto dto)
        {
            var imageName = await _imageService.SaveImageAsync(dto.Logo, "brands");

            var newBrand = new Brand
            {
                Name = dto.Name,
                Logo = imageName,
                Status = dto.Status
            };

            await _brandRepository.CreateAsync(newBrand);
            var result = await _brandRepository.CommitAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to create brand");
                return null;
            }
            _logger.LogInformation("Brand with id {id} was created", newBrand.Id);
            return newBrand;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand =  await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand is null)
            {
                _logger.LogWarning("Brand with id {id} was not found", id);
                return false;
            }
            await _imageService.DeleteImageAsync(brand.Logo, "brands");
             _brandRepository.Delete(brand);
            await _brandRepository.CommitAsync();
            _logger.LogInformation("Brand with id {id} was deleted", id);
            return true;
        }
        public async Task<bool> ChangeStatusAsync(int id)
        {
            var brand = await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand is null)
            {
                _logger.LogWarning("Brand with id {id} was not found", id);
                return false;
            }
            brand.Status = !brand.Status;
            await _brandRepository.CommitAsync();
            _logger.LogInformation("Brand with id {id} was updated", id);
            return true;
        }




        public async Task<Brand?> GetByIdAsync(int id)
        {

           var brand =  await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand is null)
            {
                _logger.LogWarning("Brand with id {id} was not found", id);
                return null;
            }
            
            _logger.LogInformation("Brand with id {id} was found", id);
            return brand;
        }

        public async Task<Brand?> UpdateAsync(int id, [FromForm] UpdateBrandDto dto)
        {
            var existingBrand =  await _brandRepository.GetOneAsync(b => b.Id == id);
            if (existingBrand is null)
            {
                _logger.LogWarning("Brand with id {id} was not found", id);
                return null;
            }
            if (dto.Name is not null)
            {
            existingBrand.Name = dto.Name;
                
            }
            if (dto.Status is not null)
            {
                existingBrand.Status = (bool)dto.Status;
            }
            if (dto.Logo is not null)
            {
                await _imageService.DeleteImageAsync(
                    existingBrand.Logo, "brands");

                var newImageName =
                    await _imageService.SaveImageAsync(dto.Logo, "brands");

                existingBrand.Logo = newImageName;
            }





            _brandRepository.Update(existingBrand);
            await _brandRepository.CommitAsync();
            _logger.LogInformation("Brand with id {id} was updated successfully", id);

            return existingBrand;
        }
    }
}
