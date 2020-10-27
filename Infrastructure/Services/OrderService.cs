using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;

namespace Infrastructure.Services
{
  public class OrderService : IOrderService
  {
    private readonly IGenericRepository<Order> _orderRepo;
    private readonly IGenericRepository<DeliveryMethod> _dmRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IBasketRepository _basketRepo;
    public OrderService(IGenericRepository<Order> orderRepo, IGenericRepository<DeliveryMethod> dmRepo, IGenericRepository<Product> productRepo, IBasketRepository basketRepo)
    {
      this._basketRepo = basketRepo;
      this._productRepo = productRepo;
      this._dmRepo = dmRepo;
      this._orderRepo = orderRepo;
    }

    public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
    {
      // get basket from the repo
      var basket = await _basketRepo.GetBasketAsync(basketId);

      // get the items from the product repo
      var items = new List<OrderItem>();
      foreach (var item in basket.Items)
      {
        var productItem = await _productRepo.GetByIdAsync(item.Id);
        var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
        var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);

        items.Add(orderItem);
      }

      // get delivery method from repo
      var deliveryMethod = await _dmRepo.GetByIdAsync(deliveryMethodId);

      // calculate the subtotal
      var subTotal = items.Sum(item => item.Price * item.Quantity);

      // create the order
      var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subTotal);

      // save it to the db
      // TODO

      // return the order
      return order;
    }

    public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    {
      throw new System.NotImplementedException();
    }

    public Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
    {
      throw new System.NotImplementedException();
    }

    public Task<IReadOnlyList<Order>> GetOrdersForUSerAsync(string buyerEmail)
    {
      throw new System.NotImplementedException();
    }
  }
}