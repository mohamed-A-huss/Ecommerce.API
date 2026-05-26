using Ecommerce.API.Services;
using Ecommerce.API.Utility;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterCategoryDto filter,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _categoryService.GetAll(filter, pageNumber, pageSize);
           
            return Ok(new CategoryResponseDto
            {   
                Categories = result.Categories,

                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                Query = filter.Name
            });
        }
        [HttpGet("{id}", Name = "GetCategoryById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null)
                return NotFound();


           
            return Ok(category);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> CreateAsync(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCategory = await _categoryService.CreateAsync(dto);
            if (createdCategory is null)
            {
                return BadRequest();
            }

            return CreatedAtRoute(
                "GetCategoryById",
                new { id = createdCategory.Id }, createdCategory);
        }
        
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedCategory = await _categoryService.UpdateAsync(id, dto);
            if (updatedCategory is null)
                return NotFound();

            return Ok(updatedCategory);
        }


        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();

        }
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> ChangeStatusAsync(int id)
        {
            var result = await _categoryService.ChangeStatusAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}