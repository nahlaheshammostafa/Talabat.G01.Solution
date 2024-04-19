using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
	internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
	{
		public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec) 
		{
			var query = inputQuery;  //_dbContext.Set<Product>()

			if(spec.Criteria is not null)  // P => P.Id == 1
				query = query.Where(spec.Criteria);

			if(spec.OrderBy is not null)  // P => P.Name
				query = query.OrderBy(spec.OrderBy);

			else if(spec.OrderByDesc is not null)
				query = query.OrderByDescending(spec.OrderByDesc);

			// query = _dbContext.Set<Product>().Where(P => P.Id == 1)
			
			query = spec.Includes.Aggregate(query,(currentQuery,includeExpression) => currentQuery.Include(includeExpression));

			// query = _dbContext.Set<Product>().Where(P => P.Id == 1).Include(P => P.Brand)
			// query = _dbContext.Set<Product>().Where(P => P.Id == 1).Include(P => P.Brand).Include(P => P.Category)

			return query;

		}
	}
}
