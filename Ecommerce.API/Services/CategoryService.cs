

using Ecommerce.API.Repositories;

namespace Ecommerce.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<PaginatedCategoryResponseDto> GetAll(FilterCategoryDto filter ,int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            string applyFilter = "Applied filters: ";
            var categories =  await _categoryRepository.GetAsync();

            if (categories.Count() == 0)
            {
                _logger.LogInformation("No categories found");
                return new PaginatedCategoryResponseDto
                {
                    Categories = Enumerable.Empty<CategoryItemDto>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0
                };
            }

            if (filter.Name is not null)
            {
                categories = categories.Where(
                    e => e.Name.ToLower().Contains(filter.Name.ToLower()));
                applyFilter += $"Name contains '{filter.Name}' ";
            }

            if (filter.Status is not null)
            {
                categories = categories.Where(e => e.Status == filter.Status);
                applyFilter += $"Status is '{filter.Status}' ";
            }

            

            _logger.LogInformation(applyFilter);
            // Pagination
            var totalCount = await categories.CountAsync();

            double totalPages = Math.Ceiling(totalCount / (double)pageSize);

            categories = categories.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var result = await categories.ToListAsync(cancellationToken);

            return new PaginatedCategoryResponseDto
            {
                Categories = result.Select(p => new CategoryItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status= p.Status,
                }),
                CurrentPage = pageNumber,
                TotalPages = (int)totalPages,
                TotalCount = totalCount
            };
        }





        public async Task<CategoryItemDto?> CreateAsync(CreateCategoryDto dto)
        {
            var newCategory = new Category
            {
                Name = dto.Name,

                Status = dto.Status,

            };

            await _categoryRepository.CreateAsync(newCategory);
            var result = await _categoryRepository.CommitAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to create category");
                return null;
            }
            _logger.LogInformation("Category with id {id} was created", newCategory.Id);
            CategoryItemDto response = new()
            {
                Id = newCategory.Id,
                Name = newCategory.Name,
                Status = newCategory.Status
            };
            return response;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category =  await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null)
            {
                _logger.LogWarning("Category with id {id} was not found", id);
                return false;
            }
             _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync();
            _logger.LogInformation("Category with id {id} was deleted", id);
            return true;
        }

        

        public async Task<CategoryItemDto?> GetByIdAsync(int id)
        {

           var category =  await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null)
            {
                _logger.LogWarning("Category with id {id} was not found", id);
                return null;
            }
            
            _logger.LogInformation("Category with id {id} was found", id);
            CategoryItemDto response = new()
            {
                Id = category.Id,
                Name = category.Name,
                Status = category.Status
            };
            return response;
        }

        public async Task<CategoryItemDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var existingCategory =  await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (existingCategory is null)
            {
                _logger.LogWarning("Category with id {id} was not found", id);
                return null;
            }
            if (dto.Name is not null)
            {
            existingCategory.Name = dto.Name;
                
            }
            if (dto.Status is not null)
            {
                existingCategory.Status = (bool)dto.Status;
            }



            _categoryRepository.Update(existingCategory);
            await _categoryRepository.CommitAsync();
            _logger.LogInformation("Category with id {id} was updated successfully", id);
            CategoryItemDto response = new()
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                Status = existingCategory.Status
            };

            return response;
        }

        public async Task<bool> ChangeStatusAsync(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null)
            {
                _logger.LogWarning("Category with id {id} was not found", id);
                return false;
            }
            category.Status = !category.Status;
            await _categoryRepository.CommitAsync();
            _logger.LogInformation("Category with id {id} was updated", id);
            return true;
        }
    }
}
