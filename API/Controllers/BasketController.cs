using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class BasketController : BaseController
  {
    private readonly IBasketRepository _basketRepository;
    public BasketController(IBasketRepository basketRepository)
    {
      this._basketRepository = basketRepository;
    }

    // api/basket?id=basket1
    [HttpGet]
    public async Task<ActionResult<CustomerBasket>> GetBasketById([FromQuery] string id)
    {
      var basket = await _basketRepository.GetBasketAsync(id);

      // if no basket exists with the id from the client, initialise a new one
      return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
    {
      var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

      return Ok(updatedBasket);
    }

    [HttpDelete]
    public async Task DeleteBasketAsync([FromQuery] string id)
    {
      await _basketRepository.DeleteBasketAsync(id);
    }
  }
}