﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;

namespace Talabat.Service.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IGenericRepository<Product> _productRepo;
		private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
		private readonly IGenericRepository<Order> _orderRepo;

		public OrderService(
			IBasketRepository basketRepo,
			IGenericRepository<Product> productRepo,
			IGenericRepository<DeliveryMethod> deliveryMethodRepo,
			IGenericRepository<Order> orderRepo) 
		{
			_basketRepo = basketRepo;
			_productRepo = productRepo;
			_deliveryMethodRepo = deliveryMethodRepo;
			_orderRepo = orderRepo;
		}
		public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
		{
			// 1. Get basket from baskets repo

			var basket = await _basketRepo.GetBasketAsync(basketId);

			// 2. Get selected items at basket from products repo

			var orderItems = new List<OrderItem>();
			if(basket?.Items?.Count > 0)
			{
				foreach(var item in basket.Items)
				{
					var product = await _productRepo.GetAsync(item.Id);
					var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
					var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
					orderItems.Add(orderItem);
				}
			}

			// 3. calculate SubTotal
			var subTotal = orderItems.Sum(item => item.Price * item.Quantity);
			// 5. Create Order
			var order = new Order(
				buyerEmail: buyerEmail,
				shippingAddress : shippingAddress,
				deliveryMethodId : deliveryMethodId,
				items : orderItems,
				subtotal: subTotal
				);
			_orderRepo.Add(order);
		}

		public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
		{
			throw new NotImplementedException();
		}

		public Task<Order> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
		{
			throw new NotImplementedException();
		}
	}
}