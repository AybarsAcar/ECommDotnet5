using System;
using System.Collections;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
  public class UnitOfWork : IUnitOfWork
  {
    private Hashtable _repositories;
    private readonly StoreContext _context;
    public UnitOfWork(StoreContext context)
    {
      this._context = context;
    }

    public async Task<int> Complete()
    {
      return await _context.SaveChangesAsync();
    }

    /* 
    default dispose method
     */
    public void Dispose()
    {
      _context.Dispose();
    }

    /* 
    check if theres an instance of a repo in the HashTable
    if not create one and add
    then return the repository from the Map
     */
    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
      if (_repositories == null) _repositories = new Hashtable();

      var type = typeof(T).Name;

      if (!_repositories.ContainsKey(type))
      {
        var repositoryType = typeof(GenericRepository<>);
        var repositoryInstance =
            Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);

        _repositories.Add(type, repositoryInstance);
      }

      return (IGenericRepository<T>)_repositories[type];
    }
  }
}

/* 
    public IRepository<T> Repository<T>() where T : class 
    { 
        if (repositories.Keys.Contains(typeof(T)) == true) 
        { 
            return repositories[typeof(T)] as IRepository<T>; 
        } 
        IRepository<T> repo = new GenericRepository<T>(entities); 
        repositories.Add(typeof(T), repo); return repo; 
    }

 */