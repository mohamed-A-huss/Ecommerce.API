using Ecommerce.API.Repositories;
using Ecommerce.API.Services;
using Ecommerce.API.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace Ecommerce.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<IBrandService, BrandService>();

            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            builder.Services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //    .AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters


            //{

            //ValidateIssuer = true,
            //ValidateAudience = true,
            //ValidateLifetime = true,
            //ValidateIssuerSigningKey = true,

            //ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            //ValidAudience = builder.Configuration["JWT:ValidAudience"],

            //IssuerSigningKey = new SymmetricSecurityKey(
            //Encoding.UTF8.GetBytes(
            //    builder.Configuration["JWT:Secret"]!
            //))

            //}; 
            //            options.Events = new JwtBearerEvents
            //{
            //    OnAuthenticationFailed = context =>
            //    {
            //        Console.WriteLine(context.Exception.Message);
            //        return Task.CompletedTask;
            //    }
            //};
            //});
            builder.Services.AddControllers();
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseSwagger();
                //app.UseSwaggerUI();
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDbInitializer>();
            service.Initialize();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
