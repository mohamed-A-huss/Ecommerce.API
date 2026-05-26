namespace Ecommerce.API.DTOs.Responses.Promotion
{
    public class PromotionItemDto
    {
        public int Id { get; set; }
        public double Discount { get; set; }
        public string Code { get; set; } = string.Empty;
        public int Usage { get; set; } 
        public DateTime ValidTo { get; set; } 

        public int? ProductId { get; set; }
    }
}
