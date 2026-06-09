using Ecommerce.API.DTOs.Requests.Account;
using Ecommerce.API.DTOs.Requests.Authentication;
using Ecommerce.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountService _accountService;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAccountService accountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto registerRequest)
        {
            var user = new ApplicationUser()
            {
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                Address = registerRequest.Address
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);


            if (!result.Succeeded)
            {


                return BadRequest(result.Errors);
            }

          
            await _accountService.SendConfirmationMailAsync(user, Url, Request);

            //await _signInManger.SignInAsync(user, false); // Automatic login

            await _userManager.AddToRoleAsync(user, SD.CUSTOMER_ROLE);

            return Ok(new APIResponse()
            {
                StatusCode = 200
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(loginRequest.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("UserNameOrEmail", "Email Or UserName Incorrect");
                ModelState.AddModelError("Password", "Password Incorrect");

                return BadRequest(ModelState);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.RememberMe, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("UserNameOrEmail", "Email Or UserName Incorrect");
                ModelState.AddModelError("Password", "Password Incorrect");

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too many attempts, please try again later");
                }

                return BadRequest(ModelState);
            }
            string? token = await _accountService.GenerateTokenAsync(user.Id, user.Email!);

            var refreshToken = _accountService.GenerateRefreshToken();
            user.RefreshToken = _accountService.HashRefreshToken(refreshToken);

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            return Ok(new
            {
                AccessToken = token,
                RefreshToken = refreshToken
            });
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var hashedToken =_accountService.HashRefreshToken(request.RefreshToken);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedToken);
            if (user is null)
            {
                return Unauthorized();
            }
            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Refresh token expired");
            }
            var newAccessToken =await _accountService.GenerateTokenAsync(user.Id,user.Email!);
            
            var newRefreshToken =_accountService.GenerateRefreshToken();
            user.RefreshToken = _accountService.HashRefreshToken(newRefreshToken);
            user.RefreshTokenExpiryTime =DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);
            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();

            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var result = await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync();

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new
            {
                Message = "Logged out successfully"
            });
        }
        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> RevokeRefreshToken()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();

            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new
            {
                Message = "Refresh token revoked successfully"
            });
        }

    } 
}
