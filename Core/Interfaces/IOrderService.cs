using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces
{
  public interface IOrderService
  {
    Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethod, string basketId, Address shippingAddress);

    Task<IReadOnlyList<Order>> GetOrdersForUSerAsync(string buyerEmail);

    Task<Order> GetOrderByIdAsync(int id, string buyerEmail);

    // for convenience
    Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
  }
}