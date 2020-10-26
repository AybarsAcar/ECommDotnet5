using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
  public interface IBasketRepository
  {
    Task<CustomerBasket> GetBasketAsync(string basketId);

    /* 
    this method will be used to create or update a basket
    */
    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);

    Task<bool> DeleteBasketAsync(string basketId);
  }
}