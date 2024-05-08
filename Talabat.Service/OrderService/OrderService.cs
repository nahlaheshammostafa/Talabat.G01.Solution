using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service.OrderService
{
	
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;

		///private readonly IGenericRepository<Product> _productRepo;
		///private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
		///private readonly IGenericRepository<Order> _orderRepo;

		public OrderService(
			IBasketRepository basketRepo,
			IUnitOfWork unitOfWork)
			///IGenericRepository<Product> productRepo,
			///IGenericRepository<DeliveryMethod> deliveryMethodRepo,
			///IGenericRepository<Order> orderRepo 
		{
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
			///_productRepo = productRepo;
			///_deliveryMethodRepo = deliveryMethodRepo;
			///_orderRepo = orderRepo;
		}
		public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
		{
			// 1. Get basket from baskets repo

			var basket = await _basketRepo.GetBasketAsync(basketId);

			// 2. Get selected items at basket from products repo

			var orderItems = new List<OrderItem>();
			if(basket?.Items?.Count > 0)
			{
				var productRepository = _unitOfWork.Repository<Product>();
				foreach (var item in basket.Items)
				{
					var product = await productRepository.GetAsync(item.Id);
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
			 _unitOfWork.Repository<Order>().Add(order);

			// 6. Save to database
			var result = await _unitOfWork.CompleteAsync();

			if (result <= 0) return null;

			return order;
		}

		public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
		{
			var ordersRepo = _unitOfWork.Repository<Order>();
			var spec = new OrderSpecifications(buyerEmail);
			var orders = await ordersRepo.GetAllWithSpecAsync(spec);
			return orders;
		}

		public Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
		{
			var orderRepo = _unitOfWork.Repository<Order>();
			var orderSpec = new OrderSpecifications(orderId, buyerEmail);
			var order = orderRepo.GetWithSpecAsync(orderSpec);
			return order;
		}

		public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
			=> await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
	}
}
