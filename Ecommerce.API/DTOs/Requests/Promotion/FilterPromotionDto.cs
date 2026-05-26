namespace Ecommerce.API.DTOs.Requests.Promotion
{
    public class FilterPromotionDto
    {
        public int? ProductId { get; set; }
        public double? Discount { get; set; }
        public string? Code { get; set; }


    }
}
