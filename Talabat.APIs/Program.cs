
namespace Talabat.APIs
{
	public class Program
	{
		//Entery Point
		public static void Main(string[] args)
		{
			var webApplicationBuilder = WebApplication.CreateBuilder(args);

			#region Configure Service
			// Add services to the DI container.

			webApplicationBuilder.Services.AddControllers();
			//Rejister Required Web APIs services to the DI container.


			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			webApplicationBuilder.Services.AddEndpointsApiExplorer();
			webApplicationBuilder.Services.AddSwaggerGen(); 
			#endregion

			var app = webApplicationBuilder.Build();

			#region Configure Kestrel Middlewares
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.MapControllers(); 
			#endregion

			app.Run();
		}
	}
}
