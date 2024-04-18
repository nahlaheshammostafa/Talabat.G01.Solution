using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi;
using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
	//By Convension
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IWebHostEnvironment _env;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env) 
		{
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext httpContent)
		{
			try
			{
				// Take an action with the request

				await _next.Invoke(httpContent);  //Go to next Middleware

				// Take an action with the response

			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);  // Development Env
				// Log Exception in (Database | Files)  // Production Env

				httpContent.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
				httpContent.Response.ContentType = "application/json";

				var response = _env.IsDevelopment() ?
					new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace)
					:
					new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

				var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
				var json = JsonSerializer.Serialize(response, options);
				await httpContent.Response.WriteAsync(json);
			}
		}
	}
}
