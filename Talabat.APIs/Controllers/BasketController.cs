using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.APIs.Controllers
{
	public class BasketController : BaseApiController
	{
		private readonly IBasketRepository _basketRepository;

		public BasketController(IBasketRepository basketRepository) 
		{
			_basketRepository = basketRepository;
		}

		[HttpGet]  //GET: / api/basket?id=
		public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
		{
			var basket = await _basketRepository.GetBasketAsync(id);
			return Ok(basket ?? new CustomerBasket(id));
		}

		[HttpPost]  //POST: /api/basket
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasket basket)
		{
			var createOrUpdateBasket = await _basketRepository.UpdateBasketAsync(basket);
			if (createOrUpdateBasket is null) return BadRequest(new ApiResponse(400));
			return Ok(createOrUpdateBasket);
		}

		[HttpDelete] //DELETE: /api/basket?id=
		public async Task DeleteBasket(string id)
		{
			await _basketRepository.DeleteBasketAsync(id);
		}
	}
}
