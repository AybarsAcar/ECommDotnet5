using System.Collections.Generic;

namespace Core.Entities
{
  public class CustomerBasket
  {
    /* 
    to avoid any problems with Redis
    so an instance can be created if we dont have the id as well
     */
    public CustomerBasket()
    {
    }

    public CustomerBasket(string id)
    {
      Id = id;
    }

    // client will generate the id
    public string Id { get; set; }

    public List<BasketItem> Items { get; set; } = new List<BasketItem>();
  }
}