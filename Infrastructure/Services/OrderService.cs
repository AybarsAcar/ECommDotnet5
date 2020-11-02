using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
  public class OrderService : IOrderService
  {
    private readonly IBasketRepository _basketRepo;
    private readonly IUnitOfWork _unit;
    private readonly IPaymentService _paymentService;
    public OrderService(IBasketRepository basketRepo, IUnitOfWork unit, IPaymentService paymentService)
    {
      this._paymentService = paymentService;
      this._unit = unit;
      this._basketRepo = basketRepo;
    }

    public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
    {
      // get basket from the repo
      var basket = await _basketRepo.GetBasketAsync(basketId);

      // get the items from the product repo
      var items = new List<OrderItem>();
      foreach (var item in basket.Items)
      {
        var productItem = await _unit.Repository<Product>().GetByIdAsync(item.Id);
        var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
        var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);

        items.Add(orderItem);
      }

      // get delivery method from repo
      var deliveryMethod = await _unit.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

      // calculate the subtotal
      var subTotal = items.Sum(item => item.Price * item.Quantity);

      // check to see if order exists with the same payment intent id
      var spec = new OrderByPaymentIntentIdSpecification(basket.PaymentIntentId);
      var existingOrder = await _unit.Repository<Order>().GetEntityWithSpec(spec);

      if (existingOrder != null)
      {
        // delete the order
        _unit.Repository<Order>().Delete(existingOrder);
        // and create another id just to make sure the order is accurete
        await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
      }

      // create the order and add it
      var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subTotal, basket.PaymentIntentId);
      _unit.Repository<Order>().Add(order);

      // save it to the db -- if this fails all the changes will roll back
      var result = await _unit.Complete();

      if (result <= 0) return null;

      // return the order
      return order;
    }

    public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    {
      return await _unit.Repository<DeliveryMethod>().ListAllAsync();
    }

    public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
    {
      var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

      return await _unit.Repository<Order>().GetEntityWithSpec(spec);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
    {
      var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

      return await _unit.Repository<Order>().ListAsync(spec);
    }
  }
}