using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.OrderAggregate
{
	public class Order : BaseEntity
	{
		private Order() { }
		public Order(string buyerEmail, Address shippingAddress, int? deliveryMethodId, ICollection<OrderItem> items, decimal subtotal, string paymentIntentId)
		{
			BuyerEmail = buyerEmail;
			ShippingAddress = shippingAddress;
			DeliveryMethodId = deliveryMethodId;
			Items = items;
			Subtotal = subtotal;
			PaymentIntentId = paymentIntentId;
		}

		public string BuyerEmail { get; set; } = null!;
		public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
		public OrderStatus Status { get; set; } = OrderStatus.Pending;
		public Address ShippingAddress { get; set; } = null!;
		public int? DeliveryMethodId { get; set; }    // Foreign Key
		public DeliveryMethod? DeliveryMethod { get; set; } = null!;   //Navigation Property [ONE]
		public ICollection<OrderItem> Items { get; set;} = new HashSet<OrderItem>();  //Navigation Property [Many]
		public decimal Subtotal { get; set; }
		public decimal GetTotal() => Subtotal + DeliveryMethod.Cost;
		public string PaymentIntentId { get; set; } = string.Empty;
	}
}
