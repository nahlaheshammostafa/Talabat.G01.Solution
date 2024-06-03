using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract;

namespace Talabat.Service.CacheService
{
	public class ResponseCacheService : IResponseCacheService
	{
		private readonly IDatabase _database;

		public ResponseCacheService(IConnectionMultiplexer redis) 
		{
			_database = redis.GetDatabase();
		}
		public async Task CacheResponseAsync(string Key, object Response, TimeSpan timeToLive)
		{
			if (Response is null) return;
			var serializeOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			var serializeResponse = JsonSerializer.Serialize(Response, serializeOptions);
			await _database.StringSetAsync(Key, serializeResponse,timeToLive);
		}

		public async Task<string?> GetCachedResponseAsync(string Key)
		{
			var response = await _database.StringGetAsync(Key);
			if(response.IsNullOrEmpty) return null;
			return response;
		}
	}
}
