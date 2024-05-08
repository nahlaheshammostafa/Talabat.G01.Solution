﻿using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Dtos
{
	public class OrderItemDto
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string ProductName { get; set; } = null!;
		public string PictureUrl { get; set; } = null!;
		public ProductItemOrdered Product { get; set; } = null!;
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}
}