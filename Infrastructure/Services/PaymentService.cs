using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;
using Product = Core.Entities.Product;

namespace Infrastructure.Services
{
  public class PaymentService : IPaymentService
  {
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unit;
    private readonly IConfiguration _config;
    public PaymentService(IBasketRepository basketRepository, IUnitOfWork unit, IConfiguration config)
    {
      this._config = config;
      this._unit = unit;
      this._basketRepository = basketRepository;
    }

    public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
    {
      StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

      var basket = await _basketRepository.GetBasketAsync(basketId);

      if (basket == null) return null;

      var shippingPrice = 0m;

      if (basket.DeliveryMethodId.HasValue)
      {
        var deliveryMethod =
            await _unit.Repository<DeliveryMethod>().GetByIdAsync((int)basket.DeliveryMethodId);

        shippingPrice = deliveryMethod.Price;
      }

      foreach (var item in basket.Items)
      {
        // get the product item from the db for each item
        var productItem = await _unit.Repository<Product>().GetByIdAsync(item.Id);

        // check for mischief from the client side
        if (item.Price != productItem.Price)
        {
          item.Price = productItem.Price;
        }
      }

      var service = new PaymentIntentService();

      PaymentIntent intent;

      // cehck if the intent is being updated or created
      if (string.IsNullOrEmpty(basket.PaymentIntentId))
      {
        var options = new PaymentIntentCreateOptions
        {
          Amount = (long)basket.Items.Sum(item => item.Quantity * (item.Price * 100)) + (long)shippingPrice * 100,
          Currency = "usd",
          PaymentMethodTypes = new List<string> { "card" }
        };

        intent = await service.CreateAsync(options);

        basket.PaymentIntentId = intent.Id;
        basket.ClientSecret = intent.ClientSecret;
      }
      else
      {
        // update the intent
        var options = new PaymentIntentUpdateOptions
        {
          Amount = (long)basket.Items.Sum(item => item.Quantity * (item.Price * 100)) + (long)shippingPrice * 100,
        };

        await service.UpdateAsync(basket.PaymentIntentId, options);
      }

      await _basketRepository.UpdateBasketAsync(basket);

      return basket;
    }

    public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
    {
      // get hold of our order
      var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
      var order = await _unit.Repository<Order>().GetEntityWithSpec(spec);

      if (order == null) return null;

      order.Status = OrderStatus.PaymentFailed;

      await _unit.Complete();

      return order;

    }

    public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
    {
      // get hold of our order
      var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
      var order = await _unit.Repository<Order>().GetEntityWithSpec(spec);

      if (order == null) return null;

      order.Status = OrderStatus.PaymentReceived;
      _unit.Repository<Order>().Update(order);

      await _unit.Complete();

      return order;
    }
  }
}