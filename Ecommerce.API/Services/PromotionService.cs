using Ecommerce.API.DTOs.Requests.Promotion;
using Ecommerce.API.DTOs.Responses.Promotion;
using Ecommerce.API.Repositories;

namespace Ecommerce.API.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly ILogger<BrandService> _logger;
        public PromotionService(IRepository<Promotion> promotionRepository, ILogger<BrandService> logger)
        {
            _promotionRepository = promotionRepository;
            _logger = logger;
        }
        public async Task<PaginatedPromotionResponseDto> GetAll(FilterPromotionDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            string applyFilter = "Applied filters: ";
            var promotions = await _promotionRepository.GetAsync();
            if (promotions.Count() == 0)
            {
                _logger.LogInformation("No promotions found");
                return new PaginatedPromotionResponseDto
                {
                    Promotions = Enumerable.Empty<PromotionItemDto>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0,                    
                    Query = filter.Code
                };
            }

            if (filter.Code is not null)
            {
                promotions = promotions.Where(
                    e => e.Code.ToLower().Contains(filter.Code.ToLower()));
                applyFilter += $"Code contains '{filter.Code}' ";
            }

            if (filter.Discount is not null)
            {
                promotions = promotions.Where(e => e.Discount == filter.Discount);
                applyFilter += $"Discount is '{filter.Discount}' ";
            }

            if (filter.ProductId is not null)
            {
                promotions = promotions.Where(e => e.ProductId == filter.ProductId);
                applyFilter += $"Product ID is '{filter.ProductId}' ";
            }



            _logger.LogInformation(applyFilter);
            // Pagination
            var totalCount = await promotions.CountAsync();

            double totalPages = Math.Ceiling(totalCount / (double)pageSize);

            promotions = promotions.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var result = await promotions.ToListAsync(cancellationToken);

            return new PaginatedPromotionResponseDto
            {
                Promotions = result.Select(p => new PromotionItemDto
                {
                    Id = p.Id,
                    Discount = p.Discount,
                    Code = p.Code,
                    Usage = p.Usage,
                    ValidTo = p.ValidTo,
                    ProductId = p.ProductId
                }),
                CurrentPage = pageNumber,
                TotalPages = (int)totalPages,
                TotalCount = totalCount,
                Query = filter.Code
            };
        }





        public async Task<PromotionItemDto?> CreateAsync(CreatePromotionDto dto)
        {
            var newPromotion = new Promotion
            {
                Discount = dto.Discount,
                Code = dto.Code,                
                ProductId = dto.ProductId
            };

              

            await _promotionRepository.CreateAsync(newPromotion);
            var result = await _promotionRepository.CommitAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to create promotion");
                return null;
            }
            _logger.LogInformation("Promotion with id {id} was created", newPromotion.Id);

            PromotionItemDto promotionItemDto = new PromotionItemDto
            {
                Id = newPromotion.Id,
                Discount = newPromotion.Discount,
                Code = newPromotion.Code,
                ProductId = newPromotion.ProductId,
                ValidTo = newPromotion.ValidTo,
            };
            return promotionItemDto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promotion = await _promotionRepository.GetOneAsync(p => p.Id == id);
            if (promotion is null)
            {
                _logger.LogWarning("Promotion with id {id} was not found", id);
                return false;
            }
            _promotionRepository.Delete(promotion);
            await _promotionRepository.CommitAsync();
            _logger.LogInformation("Promotion with id {id} was deleted", id);
            return true;
        }



        public async Task<PromotionItemDto?> GetByIdAsync(int id)
        {

            var promotion = await _promotionRepository.GetOneAsync(p => p.Id == id);
            if (promotion is null)
            {
                _logger.LogWarning("Promotion with id {id} was not found", id);
                return null;
            }

            _logger.LogInformation("Promotion with id {id} was found", id);
            PromotionItemDto promotionItemDto = new PromotionItemDto
            {
                Id = promotion.Id,
                Discount = promotion.Discount,
                Code = promotion.Code,
                ValidTo=promotion.ValidTo,
                ProductId = promotion.ProductId,
                Usage = promotion.Usage,
            };
            return promotionItemDto;
        }

        public async Task<PromotionItemDto?> UpdateAsync(int id, UpdatePromotionDto dto)
        {
            var existingPromotion = await _promotionRepository.GetOneAsync(p => p.Id == id);
            if (existingPromotion is null)
            {
                _logger.LogWarning("Promotion with id {id} was not found", id);
                return null;
            }
            if (dto.Discount is not null)
            {
                existingPromotion.Discount = (double)dto.Discount;
            }
            if (dto.Code is not null)
            {
                existingPromotion.Code = dto.Code;
            }
            if (dto.ProductId is not null)
            {
                existingPromotion.ProductId = dto.ProductId;
            }



            _promotionRepository.Update(existingPromotion);
            await _promotionRepository.CommitAsync();
            _logger.LogInformation("Promotion with id {id} was updated successfully", id);

            PromotionItemDto promotionItemDto = new PromotionItemDto
            {
                Id = existingPromotion.Id,
                Discount = existingPromotion.Discount,
                Code = existingPromotion.Code,
                ValidTo = existingPromotion.ValidTo,
                ProductId = existingPromotion.ProductId,
                Usage = existingPromotion.Usage,
            };
            return promotionItemDto;
        }
    }
}