using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class BasketController : BaseController
  {
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;
    public BasketController(IBasketRepository basketRepository, IMapper mapper)
    {
      this._mapper = mapper;
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
    public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasketDto basketDto)
    {
      var basket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basketDto);

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