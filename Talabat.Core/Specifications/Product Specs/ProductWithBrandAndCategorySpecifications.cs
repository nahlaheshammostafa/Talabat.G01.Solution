using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
	public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
	{
		// This Constructor will be used for creating an object, That will be used to get all Products
		public ProductWithBrandAndCategorySpecifications(string? sort,int? brandId, int? categoryId) 
			: base(P => 
			            (!brandId.HasValue || P.BrandId == brandId.Value) &&
			            (!categoryId.HasValue || P.CategoryId == categoryId.Value)
				  )
		{
			Includes.Add(P => P.Brand);
			Includes.Add(P => P.Category);

			if(!string.IsNullOrEmpty(sort))
			{
				switch (sort)
				{
					case "priceAsc":
						AddOrderBy(P => P.Price);
						break;
					case "priceDesc":
						AddOrderByDesc(P => P.Price);
						break;
					default:
						AddOrderBy(P => P.Name);
						break;
				}
			}
			else
				AddOrderBy(P => P.Name);
		}

		// This Constructor will be used for creating an object, That will be used to get specific product by id

		public ProductWithBrandAndCategorySpecifications(int id)
			:base(P => P.Id == id)
		{
			Includes.Add(P => P.Brand);
			Includes.Add(P => P.Category);
		}
	}
}
