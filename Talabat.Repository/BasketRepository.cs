using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using StackExchange.Redis;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Talabat.Repository
{
	public class BasketRepository : IBasketRepository
	{
		private readonly IDatabase _database;
		public BasketRepository(IConnectionMultiplexer redis) 
		{
			_database = redis.GetDatabase();
		}
		public async Task<bool> DeleteBasketAsync(string basketId)
		{
			return await _database.KeyDeleteAsync(basketId);
		}

		public async Task<CustomerBasket?> GetBasketAsync(string basketId)
		{
			var bsket = await _database.StringGetAsync(basketId);
			return basketId.IsNullOrEmpty() ? null : JsonSerializer.Deserialize<CustomerBasket>(bsket);
		}

		public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
		{
			var createOrUpdate = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));
			if (createOrUpdate is false) return null;
			return await GetBasketAsync(basket.Id);
		}
	}
}
