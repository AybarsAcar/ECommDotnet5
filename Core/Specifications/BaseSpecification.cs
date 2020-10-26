using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Specifications
{
  public class BaseSpecification<T> : ISpecification<T>
  {
    // constructor for no criteria (i.e. no use of filtering)
    public BaseSpecification()
    {
    }

    // constructor for spec with filtering
    public BaseSpecification(Expression<Func<T, bool>> criteria)
    {
      Criteria = criteria;
    }

    public Expression<Func<T, bool>> Criteria { get; }

    public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

    public Expression<Func<T, object>> OrderBy { get; private set; }

    public Expression<Func<T, object>> OrderByDescending { get; private set; }

    public int Take { get; private set; }

    public int Skip { get; private set; }

    public bool IsPagingEnabled { get; private set; }

    /* 
    method to allow us to add inlude statements to our List of expressions
     */
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
      Includes.Add(includeExpression);
    }

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
      OrderBy = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
      OrderByDescending = orderByDescExpression;
    }

    /* 
    Pagination
     */
    protected void ApplyPaging(int skip, int take)
    {
      Skip = skip;
      Take = take;
      IsPagingEnabled = true;
    }
  }
}