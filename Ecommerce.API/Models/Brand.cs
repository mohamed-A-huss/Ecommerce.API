
namespace Ecommerce.API.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Logo { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}
