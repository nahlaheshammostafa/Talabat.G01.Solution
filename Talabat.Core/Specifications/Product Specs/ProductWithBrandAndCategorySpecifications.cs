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
		public ProductWithBrandAndCategorySpecifications() 
			: base()
		{
			Includes.Add(P => P.Brand);
			Includes.Add(P => P.Category);
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
