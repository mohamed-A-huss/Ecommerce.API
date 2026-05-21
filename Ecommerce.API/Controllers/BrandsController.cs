namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        [HttpGet]
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
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand is null)
                return NotFound();


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
        public async Task<IActionResult> CreateAsync([FromForm] CreateBrandDto dto)
        {
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
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateBrandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedBrand = await _brandService.UpdateAsync(id, dto);
            if (updatedBrand is null)
                return NotFound();

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
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _brandService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();

        }
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatusAsync(int id)
        {
            var result = await _brandService.ChangeStatusAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}