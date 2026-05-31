using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTPRepository, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
            _configuration = configuration;
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
        public async Task<string?> GenerateTokenAsync(string userId, string email)
        {
            //var jwt = _configuration.GetSection("Jwt");
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return null;

            List<Claim> claims = new List<Claim>();

            claims.Add(new(ClaimTypes.NameIdentifier, userId));
            claims.Add(new(ClaimTypes.Email, email));
            claims.Add(
            new Claim(
                     JwtRegisteredClaimNames.Iat,
                     DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                     ClaimValueTypes.Integer64));

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var item in userRoles)
            {
                claims.Add(new(ClaimTypes.Role, item));
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
            SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
