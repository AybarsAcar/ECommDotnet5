using System.Linq;
using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
  public class SpecificationEvaluator<T> where T : BaseEntity
  {
    /* 
    Ordering is important
    Check for filtering first
    and apply the Paging the last when building up the query
     */
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
      var query = inputQuery;

      // evaluate whats inside specification
      // this is our like Where expressions
      if (spec.Criteria != null)
      {
        query = query.Where(spec.Criteria);
      }

      if (spec.OrderBy != null)
      {
        query = query.OrderBy(spec.OrderBy);
      }

      if (spec.OrderByDescending != null)
      {
        query = query.OrderByDescending(spec.OrderByDescending);
      }

      // paging
      if (spec.IsPagingEnabled)
      {
        query = query.Skip(spec.Skip).Take(spec.Take);
      }

      // takes the include statements aggregate them and pass into our query
      query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

      return query;
    }
  }
}