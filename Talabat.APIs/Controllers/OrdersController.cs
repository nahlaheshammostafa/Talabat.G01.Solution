﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
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

		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost]  // POST: /api/Orders
		public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
		{
			var address = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
			var order = await _orderService.CreateOrderAsync(orderDto.BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, address);
			if (order is null) return BadRequest(new ApiResponse(400));
			return Ok(order);
		}


		[HttpGet] // GET: /api/Orders?email=nahla@gmail.com
		public async Task<ActionResult<IReadOnlyList<Order>>> GetOrderForUser(string email)
		{
			var orders = await _orderService.GetOrdersForUserAsync(email);
			return Ok(orders);
		}

		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpGet("{id}")]  // GET : /api/Orders/1?email=nahla@gmail.com
		public async Task<ActionResult<Order>> GetOrderForUser(int id, string email)
		{
			var order = await _orderService.GetOrderByIdForUserAsync(id, email);
			if (order is null) return NotFound(new ApiResponse(404));
			return Ok(order);
		}


	}
}
