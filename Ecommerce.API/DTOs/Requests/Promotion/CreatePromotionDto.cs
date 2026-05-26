namespace Ecommerce.API.DTOs.Requests.Promotion
{
    public class CreatePromotionDto
    {
        public double Discount { get; set; }
        public string Code { get; set; } = string.Empty;
        public int? ProductId { get; set; }
    }
}
