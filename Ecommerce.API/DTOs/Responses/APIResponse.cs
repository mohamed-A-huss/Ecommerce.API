namespace Ecommerce.API.DTOs.Responses
{
    public class APIResponse
    {
        public string UUID { get; set; } = Guid.NewGuid().ToString();
        public int StatusCode { get; set; }
        public string[]? Message { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
