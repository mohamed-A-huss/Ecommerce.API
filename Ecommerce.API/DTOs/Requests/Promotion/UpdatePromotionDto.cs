namespace Ecommerce.API.DTOs.Requests.Promotion
{
    public class UpdatePromotionDto
    {
        public double? Discount { get; set; }
        public string? Code { get; set; } = string.Empty;
        public int? ProductId { get; set; }
    }
}
