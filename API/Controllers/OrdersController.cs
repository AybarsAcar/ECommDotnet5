using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Authorize]
  public class OrdersController : BaseController
  {
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;
    public OrdersController(IOrderService orderService, IMapper mapper)
    {
      this._mapper = mapper;
      this._orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
    {
      // get the email from jwt claims
      var email = HttpContext.User.RetrieveEmailFromPrinciple();

      var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

      var order =
        await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);

      if (order == null) return BadRequest(new ApiResponse(400, "Problem creating order"));

      return Ok(order);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
    {
      // get the email from jwt claims
      var email = HttpContext.User.RetrieveEmailFromPrinciple();
      var orders = await _orderService.GetOrdersForUserAsync(email);

      var ordersToReturn = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);
      return Ok(ordersToReturn);
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int orderId)
    {
      var email = HttpContext.User.RetrieveEmailFromPrinciple();

      var order = await _orderService.GetOrderByIdAsync(orderId, email);

      if (order == null) return NotFound(new ApiResponse(404, "Order does not exist"));

      var orderToReturn = _mapper.Map<Order, OrderToReturnDto>(order);
      return orderToReturn;
    }

    [HttpGet("deliveryMethods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
      var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();
      return Ok(deliveryMethods);
    }
  }
}