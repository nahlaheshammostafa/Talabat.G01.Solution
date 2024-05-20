using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Dtos
{
	public class OrderToReturnDto
	{
		public int Id { get; set; }
		public string BuyerEmail { get; set; } = null!;
		public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
		public string Status { get; set; }
		public decimal DeliverMethodCost  { get; set; }
		public Address ShippingAddress { get; set; } = null!;

		public int? DeliveryMethodId { get; set; }    // Foreign Key
		public string DeliveryMethod { get; set; } = null!;   //Navigation Property [ONE]
		public ICollection<OrderItemDto> Items { get; set; } = new HashSet<OrderItemDto>();  //Navigation Property [Many]
		public decimal Subtotal { get; set; }
		public decimal Total { get; set; }
		public string PaymentIntentId { get; set; } = string.Empty;
	}
}
