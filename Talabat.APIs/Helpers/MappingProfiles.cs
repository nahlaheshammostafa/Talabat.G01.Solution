using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.OrderAggregate;
using Address = Talabat.Core.Entities.Identity.Address;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles() 
		{
			CreateMap<Product, ProductToReturnDto>()
				.ForMember(P => P.Brand, O => O.MapFrom(S => S.Brand.Name))
				.ForMember(P => P.Category, O => O.MapFrom(S => S.Category.Name))
				.ForMember(P => P.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

			CreateMap<CustomerBasketDto, CustomerBasket>();
			CreateMap<BasketItemDto, BasketItem>();
			CreateMap<Address, AddressDto>().ReverseMap();

			CreateMap<AddressDto, Talabat.Core.Entities.OrderAggregate.Address>();

			CreateMap<Order, OrderToReturnDto>()
				.ForMember(d => d.DeliveryMethod, O => O .MapFrom(S => S.DeliveryMethod.ShortName))
			    .ForMember(d => d.DeliverMethodCost, O => O.MapFrom(S => S.DeliveryMethod.Cost));

			CreateMap<OrderItem, OrderItemDto>()
				.ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
				.ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
				.ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
				.ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureResolver>());
			    



		}
	}
}
