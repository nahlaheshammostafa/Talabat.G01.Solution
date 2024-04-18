
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;

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
			//Rejister Required Web APIs services to the DI container.


			webApplicationBuilder.Services.AddSwaggerServices();

			webApplicationBuilder.Services.AddDbContext<StoreContext>(options => 
			{
				options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
			});

			webApplicationBuilder.Services.AddApplicationServices();
			#endregion

			var app = webApplicationBuilder.Build();

			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;
			var _dbContext = services.GetRequiredService<StoreContext>();
			// Ask CLR for creating object from DbContext Explicitly

			var loggerFactory = services.GetRequiredService<ILoggerFactory>();
			try
			{
				await _dbContext.Database.MigrateAsync();
				await StoreContextSeed.SeedAsync(_dbContext); //DataSeeding
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
