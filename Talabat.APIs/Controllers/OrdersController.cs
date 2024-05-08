using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class OrdersController : BaseApiController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;

		public OrdersController(
			IOrderService orderService,
			IMapper mapper)
		{
			_orderService = orderService;
			_mapper = mapper;
		}

		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost]  // POST: /api/Orders
		public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
		{
			var address = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
			var order = await _orderService.CreateOrderAsync(orderDto.BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, address);
			if (order is null) return BadRequest(new ApiResponse(400));
			return Ok(_mapper.Map<Core.Entities.OrderAggregate.Order, OrderToReturnDto>(order));
		}


		[HttpGet] // GET: /api/Orders?email=nahla@gmail.com
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrderForUser(string email)
		{
			var orders = await _orderService.GetOrdersForUserAsync(email);
			return Ok(_mapper.Map<IReadOnlyList<Core.Entities.OrderAggregate.Order>,IReadOnlyList<OrderToReturnDto>>(orders));
		}

		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpGet("{id}")]  // GET : /api/Orders/1?email=nahla@gmail.com
		public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id, string email)
		{
			var order = await _orderService.GetOrderByIdForUserAsync(id, email);
			if (order is null) return NotFound(new ApiResponse(404));
			return Ok(_mapper.Map<Core.Entities.OrderAggregate.Order, OrderToReturnDto>(order));
		}

		[Authorize]
		[HttpGet("deliveryMethods")]  // GET : /api/Orders/deliveryMethods
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
		{
			var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();
			return Ok(deliveryMethods);
		}

	}
}
