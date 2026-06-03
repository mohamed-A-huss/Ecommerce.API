using Ecommerce.API.DTOs.Requests.User;
using Ecommerce.API.DTOs.Responses.User;
using Ecommerce.API.Services;
using Ecommerce.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> GetUsers(
                                                [FromQuery] FilterUserDto filter,
                                                int page = 1,
                                                int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var Currentuser = await _userManager.GetUserAsync(User);
            if (Currentuser is null) return Unauthorized();
            var users = _userManager.Users.AsQueryable();

            // Filter
            if (filter.FirstName is not null)
            {
                users = users.Where(e => e.FirstName.Contains(filter.FirstName));
            }

            if (filter.LastName is not null)
            {
                users = users.Where(e => e.LastName.Contains(filter.LastName));
            }

            // Pagination
            double totalPages = Math.Ceiling(users.Count() / (double)pageSize);

            var userList = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Mapping
            List<UserWithRoleDto> result = new();

            foreach (var item in userList)
            {
                var role = (await _userManager.GetRolesAsync(item))
                    .FirstOrDefault();

                result.Add(new UserWithRoleDto
                {
                    UserName = item.UserName!,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    PhoneNumber = item.PhoneNumber!,
                    Email = item.Email!,
                    Role = role ?? "No Role"
                });
            }

            return Ok(new ApplicationUserResponseDto()
            {
                Users = result,
                TotalPages = totalPages,
                CurrentPage = page,
            });
        }
        
        [HttpGet("GetUserByEmail")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> GetByIdAsync(string email)
        {
            var Currentuser = await _userManager.GetUserAsync(User);
            if (Currentuser is null) return Unauthorized();
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, SD.SUPER_ADMIN_ROLE))
                return BadRequest("Cannot view Super Admin details");


            var userItemDto = new UserItemDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Email = email,

            };
            return Ok(userItemDto);
        }



        [HttpPut("UpdateRole")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> UpdateRole(string Email,UpdateUserRoleDto dto)
        {
            
            var Currentuser = await _userManager.GetUserAsync(User);
            if (Currentuser is null) return Unauthorized();
            var user = await _userManager.FindByEmailAsync(Email);

            if (user is null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, SD.SUPER_ADMIN_ROLE))
                return BadRequest("Cannot change Super Admin role");

            var roleExists = await _roleManager.RoleExistsAsync(dto.RoleName);

            if (!roleExists)
                return BadRequest("Role does not exist");

            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);

            await _userManager.AddToRoleAsync(user, dto.RoleName);

            return Ok(new
            {
                Message = "Role updated successfully"
            });
        }
        [HttpPut("LockUnlock")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]

        public async Task<IActionResult> LockUnLock(string Email)
        {
            
            var Currentuser = await _userManager.GetUserAsync(User);
            if (Currentuser is null) return Unauthorized();
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null) return NotFound();

            user.LockoutEnabled = !user.LockoutEnabled;

            if (!user.LockoutEnabled)
            {
                user.LockoutEnd = DateTime.Now.AddDays(14);
            }
            else
            {
                user.LockoutEnd = null;
            }

            await _userManager.UpdateAsync(user);

            return Ok(new UserItemDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Email = user.Email!
            });
        }
        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "No Role";

            var userItemDto = new UserItemDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Email = user.Email!,

            };
            return Ok(userItemDto);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();
            if (dto.FirstName != null)
            {
                user.FirstName = dto.FirstName;
            }
            if(dto.LastName != null)
            {
                user.LastName = dto.LastName;
            }
            if(dto.Email != null)
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);

                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return BadRequest("Email already exists");
                }
                var emailResult = await _userManager.SetEmailAsync(user, dto.Email);

                if (!emailResult.Succeeded)
                {
                    return BadRequest(emailResult.Errors.Select(e => e.Description));
                }
            }
            
            if(dto.Address != null)
            {
                user.Address = dto.Address;
            }
            if (dto.PhoneNumber != null)
            {
                var phoneResult = await _userManager.SetPhoneNumberAsync(user, dto.PhoneNumber);
                if (!phoneResult.Succeeded)
                {
                    return BadRequest(phoneResult.Errors.Select(e => e.Description));
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new UserItemDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email!,
                Address = user.Address
            });
        }
        [HttpPut("Password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();
            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded) {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new
            {
                Message = "Password updated successfully"
            });
        }
    }
}
