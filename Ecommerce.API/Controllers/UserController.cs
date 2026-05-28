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
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]

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
                    Id = item.Id,
                    UserName = item.UserName!,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
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
        
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var Currentuser = await _userManager.GetUserAsync(User);
            if (Currentuser is null) return Unauthorized();
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();




            var userItemDto = new UserItemDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,

            };
            return Ok(userItemDto);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id,UpdateUserRoleDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var Currentuser = await _userManager.GetUserAsync(User);
            if (Currentuser is null) return Unauthorized();
            var user = await _userManager.FindByIdAsync(id);

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
        [HttpPut("{id}/lock")]

        public async Task<IActionResult> LockUnLock(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var Currentuser = await _userManager.GetUserAsync(User);
            if (Currentuser is null) return Unauthorized();
            var user = await _userManager.FindByIdAsync(id);
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

            return Ok(user);
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

            return Ok($"Authenticated: {user.UserName}, Email: {user.Email}, First Name: {user.FirstName}, Last Name: {user.LastName}, Role: {role}");
        }
    }
}
