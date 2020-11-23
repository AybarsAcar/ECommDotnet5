using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class AccountController : BaseController
  {
    private readonly UserManager<AppUser> _userManger;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    public AccountController(UserManager<AppUser> userManger, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
    {
      this._mapper = mapper;
      this._tokenService = tokenService;
      this._signInManager = signInManager;
      this._userManger = userManger;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
      // get the email from the Http context -- and the user object from the claim principle
      var user = await _userManger.FindByEmailFromClaimsPrinciple(HttpContext.User);

      return new UserDto
      {
        Email = user.Email,
        Token = _tokenService.CreateToken(user),
        DisplayName = user.DisplayName
      };
    }

    /* 
    helper method for the client
    for async validation on the client side
     */
    [HttpGet("emailexists")]
    public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
      return await _userManger.FindByEmailAsync(email) != null;
    }

    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<AddressDto>> GetUserAddress()
    {
      // get the user
      var user = await _userManger.FindUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

      return _mapper.Map<Address, AddressDto>(user.Address);
    }

    /* 
    both to update and create an address initially
     */
    [HttpPut("address")]
    public async Task<ActionResult<AddressDto>> UpdateUserAddress([FromBody] AddressDto address)
    {
      // get the user from Http context jwt claims
      var user = await _userManger.FindUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

      // automapper to update the properties
      user.Address = _mapper.Map<AddressDto, Address>(address);

      // update the user
      var result = await _userManger.UpdateAsync(user);

      if (result.Succeeded) return Ok(_mapper.Map<Address, AddressDto>(user.Address));

      return BadRequest("Problem updating the user address");
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
      // user manager to get the user from db
      var user = await _userManger.FindByEmailAsync(loginDto.Email);

      if (user == null) return Unauthorized(new ApiResponse(401, "Invalid user credentials"));

      // sign in manager to check the password against whats stored to log in the user
      var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

      if (!result.Succeeded) return Unauthorized(new ApiResponse(401, "Invalid user credentials"));

      return new UserDto
      {
        Email = user.Email,
        Token = _tokenService.CreateToken(user),
        DisplayName = user.DisplayName
      };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
      // check if the email already exist
      if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
      {
        return new BadRequestObjectResult(new ApiValidationErrorResponse
        {
          Errors = new[] { "Email address is already in use" }
        });
      }

      var user = new AppUser
      {
        DisplayName = registerDto.DisplayName,
        Email = registerDto.Email,
        UserName = registerDto.Email
      };

      var result = await _userManger.CreateAsync(user, registerDto.Password);

      if (!result.Succeeded) return BadRequest(new ApiResponse(400));

      return new UserDto
      {
        Email = user.Email,
        Token = _tokenService.CreateToken(user),
        DisplayName = user.DisplayName
      };
    }
  }
}