using System;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
  public interface IUnitOfWork : IDisposable
  {
    IGenericRepository<T> Repository<T>() where T : BaseEntity;

    /* 
    return the number of changes to our database
     */
    Task<int> Complete();
  }
}