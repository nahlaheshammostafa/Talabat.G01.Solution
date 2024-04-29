
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Repository;
using Talabat.Repository._Identity;
using Talabat.Repository.Data;
using Talabat.Service.AuthService;

namespace Talabat.APIs
{
	public class Program
	{
		//Entery Point
		public static async Task Main(string[] args)
		{
			var webApplicationBuilder = WebApplication.CreateBuilder(args);

			#region Configure Service
			// Add services to the DI container.

			webApplicationBuilder.Services.AddControllers();

			webApplicationBuilder.Services.AddSwaggerServices();

			webApplicationBuilder.Services.AddApplicationServices();

			webApplicationBuilder.Services.AddDbContext<StoreContext>(options => 
			{
				options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
			});

			webApplicationBuilder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
			{
				options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
			});

			webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>

			{
				var connection = webApplicationBuilder.Configuration.GetConnectionString("Redis");
				return ConnectionMultiplexer.Connect(connection);
			});

			webApplicationBuilder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				//options.Password.RequiredUniqueChars = 2;
				//options.Password.RequireDigit = true;
				//options.Password.RequireLowercase = true;
				//options.Password.RequireUppercase = true;
			})
				.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			//webApplicationBuilder.Services.AddScoped(typeof(IAuthService),typeof(AuthService));

			webApplicationBuilder.Services.AddAuthServices(webApplicationBuilder.Configuration);

			#endregion

			var app = webApplicationBuilder.Build();

			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;
			var _dbContext = services.GetRequiredService<StoreContext>();
			var _identityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();
			// Ask CLR for creating object from DbContext Explicitly

			var loggerFactory = services.GetRequiredService<ILoggerFactory>();
			try
			{
				await _dbContext.Database.MigrateAsync();   //Update Database
				await StoreContextSeed.SeedAsync(_dbContext); //DataSeeding

				await _identityDbContext.Database.MigrateAsync(); //Update Database
				var _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
				await ApplicationIdentityContextSeed.SeedUsersAsync(_userManager);
				
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex, "an error has been occured during apply the migration");
			}

			#region Configure Kestrel Middlewares

			app.UseMiddleware<ExceptionMiddleware>();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwaggerMiddlewares();
			}

			app.UseStatusCodePagesWithReExecute("/errors/{0}");

			app.UseHttpsRedirection();

			app.MapControllers(); 

			app.UseStaticFiles();
			#endregion

			app.Run();
		}
	}
}
