using System.Linq;
using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
  public static class ApplicationServicesExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      services.AddScoped<ITokenService, TokenService>();

      services.AddScoped<IOrderService, OrderService>();

      services.AddScoped<IUnitOfWork, UnitOfWork>();

      services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

      services.AddScoped<IBasketRepository, BasketRepository>();

      services.AddScoped<IProductRepository, ProductRepository>();

      // configure the error response as an array of errors -- not a dictionary
      services.Configure<ApiBehaviorOptions>(opt =>
      {
        opt.InvalidModelStateResponseFactory = actionContext =>
        {
          var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage)
            .ToArray();

          var errorResponse = new ApiValidationErrorResponse
          {
            Errors = errors
          };

          return new BadRequestObjectResult(errorResponse);
        };
      });

      return services;
    }
  }
}