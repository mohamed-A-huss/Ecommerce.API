namespace Ecommerce.API.Interfaces
{
    public interface IAccountService
    {
        Task<bool> SendConfirmationMailAsync(ApplicationUser user, IUrlHelper url, HttpRequest request);
        Task<bool> SendOTPToMailAsync(ApplicationUser user);
    }
}
