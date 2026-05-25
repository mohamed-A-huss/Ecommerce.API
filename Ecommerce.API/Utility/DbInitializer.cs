using Microsoft.AspNetCore.Identity;

namespace Ecommerce.API.Utility
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext context, ILogger<DbInitializer> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                    _context.Database.Migrate();

                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new(SD.SUPER_ADMIN_ROLE));
                    await _roleManager.CreateAsync(new(SD.ADMIN_ROLE));
                    await _roleManager.CreateAsync(new(SD.EMPLOYEE_ROLE));
                    await _roleManager.CreateAsync(new(SD.CUSTOMER_ROLE));

                    await _userManager.CreateAsync(new()
                    {
                        Email = "SuperAdmin@eraasoft.com",
                        EmailConfirmed = true,
                        FirstName = "Super",
                        LastName = "Admin",
                        UserName = "SuperAdmin",
                    }, "Admin123$");

                    var user = await _userManager.FindByEmailAsync("SuperAdmin@eraasoft.com");

                    await _userManager.AddToRoleAsync(user!, SD.SUPER_ADMIN_ROLE);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
            }
        }
    }
}
