using Ecommerce.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BrandsController(IBrandService brandService, UserManager<ApplicationUser> userManager)
        {
            _brandService = brandService;
            _userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterBrandDto filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _brandService.GetAll(filter, pageNumber, pageSize);
           
            return Ok(new BrandResponseDto
            {
                Brands = result.Brands,

                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                Query = filter.Name
            });
        }
        [HttpGet("{id}", Name = "GetBrandById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand is null)
                return Unauthorized();


            BrandItemDto brandItemDto = new BrandItemDto
            {
                Id = brand.Id,
                Name = brand.Name,
                Status = brand.Status,
                Logo = brand.Logo
            };
            return Ok(brandItemDto);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateBrandDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);            
            Brand? createdBrand = await _brandService.CreateAsync(dto);

            if (createdBrand is null)
            {
                return BadRequest();
            }


            BrandItemDto response = new()
            {
                Id = createdBrand.Id,
                Name = createdBrand.Name,
                Status = createdBrand.Status,
                Logo = createdBrand.Logo
            };

            return CreatedAtRoute(
                "GetBrandById",
                new { id = response.Id },response);}
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateBrandDto dto)
        {
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();
            var updatedBrand = await _brandService.UpdateAsync(id, dto);
            if (updatedBrand is null)
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(new BrandItemDto
            {
                Id = updatedBrand.Id,
                Name = updatedBrand.Name,
                Status = updatedBrand.Status,
                Logo = updatedBrand.Logo

            });
        }
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> DeleteAsync(int id)

        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();
            var result = await _brandService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();

        }
        [HttpPatch("{id}/status")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> ChangeStatusAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _brandService.ChangeStatusAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}