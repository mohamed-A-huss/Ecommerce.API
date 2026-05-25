using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ecommerce.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        public async Task<bool> SendConfirmationMailAsync(ApplicationUser user, IUrlHelper url, HttpRequest request)
        {
            try
            {
                // Send Email Confirmation
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); // By Default => token valid to 24h
                var link = url.Action("Confirm", "Account", new { area = "Identity", token = token, userId = user.Id }, request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Conirmation Your account",
                    $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendOTPToMailAsync(ApplicationUser user)
        {
            try
            {
                // Send Email Confirmation
                var otp = new Random().Next(100000, 999999);
                //new Guid().ToString().Substring(0, 5);

                await _emailSender.SendEmailAsync(user.Email, $"Reset Password Your Account - {DateTime.Now}",
                    $"<h1>OTP: <b>{otp}</b>. Don't share it.<h1>");

                await _applicationUserOTPRepository.CreateAsync(new ApplicationUserOTP()
                {
                    OTP = otp.ToString(),
                    ApplicationUserId = user.Id
                });
                await _applicationUserOTPRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
