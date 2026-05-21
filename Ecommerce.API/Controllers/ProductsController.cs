using Ecommerce.API.Services;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterProductDto filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productService.GetAll(filter, pageNumber, pageSize);
           
            return Ok(new ProductResponseDto
            {
                Products = result.Products,

                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                Query = filter.Name
            });
        }
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null)
                return NotFound();


            
            return Ok(product);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Product? createdProduct = await _productService.CreateAsync(dto);
            if (createdProduct is null)
            {
                return BadRequest();
            }
            return CreatedAtRoute(
                "GetProductById",
                new { id = createdProduct.Id },
            createdProduct);
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedProduct = await _productService.UpdateAsync(id, dto);
            if (updatedProduct is null)
                return NotFound();

            return Ok(updatedProduct);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();

        }
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> ChangeStatusAsync(int id)
        {
            var result = await _productService.ChangeStatusAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}