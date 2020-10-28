using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
  /* 
  Generic repository
  only takes BaseEntity as a type param
   */
  public interface IGenericRepository<T> where T : BaseEntity
  {
    Task<T> GetByIdAsync(int id);

    Task<IReadOnlyList<T>> ListAllAsync();

    Task<T> GetEntityWithSpec(ISpecification<T> spec);

    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

    /* 
    count the number of items with specifications to apply the filters
     */
    Task<int> CountAsync(ISpecification<T> spec);

    /* 
    the following methods happen in memory
    to keep track of the changes
    UoW implement the changes to db
     */
    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
  }
}