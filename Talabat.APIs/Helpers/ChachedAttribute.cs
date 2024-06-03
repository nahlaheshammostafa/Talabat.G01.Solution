using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Mime;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Helpers
{
	public class ChachedAttribute : Attribute, IAsyncActionFilter
	{
		private readonly int _timeToLiveInSeconds;

		public ChachedAttribute(int timeToLiveInSeconds) 
		{
			_timeToLiveInSeconds = timeToLiveInSeconds;
		}	
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
			// Ask CLR for creating object from "ResponseCacheService" Explicity

			var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

			var response = await responseCacheService.GetCachedResponseAsync(cacheKey);

			if(!string.IsNullOrEmpty(response))
			{
				var result = new ContentResult()
				{
					Content = response,
					ContentType = "application/json",
					StatusCode = 200,
				};
				context.Result = result;
				return;
			}   //Response is not cached

			var executedActionContext = await next.Invoke(); // will execute the next action filter

			if(executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
			{
				await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value,TimeSpan.FromSeconds(_timeToLiveInSeconds));
			}
		}

		private string GenerateCacheKeyFromRequest(HttpRequest request)
		{
			var KeyBuilder = new StringBuilder();
			KeyBuilder.Append(request.Path);

			foreach(var (key, value) in request.Query)
			{
				KeyBuilder.Append($"|{key}-{value}");
			}
			return KeyBuilder.ToString();
		}
	}
}
