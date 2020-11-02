using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces
{
  public interface IPaymentService
  {
    Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);

    // to handle the events we listen on Stipe API
    Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);

    Task<Order> UpdateOrderPaymentFailed(string paymentIntentId);
  }
}