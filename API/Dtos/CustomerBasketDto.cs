using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
  public class CustomerBasketDto
  {
    // client will generate the id
    [Required]
    public string Id { get; set; }

    public List<BasketItemDto> Items { get; set; }

    public int? DeliveryMethodId { get; set; }
    public decimal? ShippingPrice { get; set; }

    // stripe related
    public string ClientSecret { get; set; }
    public string PaymentIntentId { get; set; }
  }
}