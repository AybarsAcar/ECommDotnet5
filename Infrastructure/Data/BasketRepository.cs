using System;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Data
{
  public class BasketRepository : IBasketRepository
  {
    // dependency injections to interact with the Redis server
    private readonly IDatabase _database;
    public BasketRepository(IConnectionMultiplexer redis)
    {
      this._database = redis.GetDatabase();
    }

    public async Task<bool> DeleteBasketAsync(string basketId)
    {
      return await _database.KeyDeleteAsync(basketId);
    }

    public async Task<CustomerBasket> GetBasketAsync(string basketId)
    {
      // get the data from the redis db whihc is stored as a string
      var data = await _database.StringGetAsync(basketId);

      return data.IsNullOrEmpty
        ? null
        : JsonSerializer.Deserialize<CustomerBasket>(data);
    }

    /* 
    this method will be used to create or update a basket
    the whole basket will be updated which is stored as a string
    */
    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
      var created = await _database.StringSetAsync(
          basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30)
        );

      // check if the basket is created or not
      if (!created) return null;

      return await GetBasketAsync(basket.Id);
    }
  }
}