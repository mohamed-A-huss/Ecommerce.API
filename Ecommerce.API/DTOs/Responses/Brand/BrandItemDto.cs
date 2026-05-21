namespace Ecommerce.API.DTOs.Responses.Brand
{
    public class BrandItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Status { get; set; } 
        public string Logo { get; set; } = string.Empty;

    }
}
