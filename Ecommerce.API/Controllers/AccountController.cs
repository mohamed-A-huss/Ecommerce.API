using Ecommerce.API.DTOs.Requests.Authentication;
using Ecommerce.API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountService _accountService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAccountService accountService)
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
                string token = null; //await _jWTHandler.GenerateTokenAsync(user.Id, user.Email!);
                return Ok(new APIResponse()
                {
                    StatusCode = 200,
                    Message = [$"Welcome Back {user.FirstName} {user.LastName}", token ?? "no-token"]
                });
            }

        } 
}
