
namespace Ecommerce.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}
