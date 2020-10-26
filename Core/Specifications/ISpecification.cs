using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Specifications
{
  /* 
  our specification expressions
   */
  public interface ISpecification<T>
  {
    /* 
    to store our filtering expressions
     */
    Expression<Func<T, bool>> Criteria { get; }

    /* 
    to store our loading expressions
    this will have list of .Include statements
     */
    List<Expression<Func<T, object>>> Includes { get; }

    /* 
    Ordering methods
     */
    Expression<Func<T, object>> OrderBy { get; }

    Expression<Func<T, object>> OrderByDescending { get; }

    /* 
    Pagination
     */
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
  }
}