using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
  /* 
  decorator for our end points that return results form the db
    decorator allows the endpoints to return the data from cache
    if not cache for future use
    used in Products Controller
   */
  public class CachedAttribute : Attribute, IAsyncActionFilter
  {
    private readonly int _timeToLiveSeconds;
    public CachedAttribute(int timeToLiveSeconds)
    {
      this._timeToLiveSeconds = timeToLiveSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var cacheService =
        context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

      // generate a key for the redis database to identify
      var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
      var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

      if (!string.IsNullOrEmpty(cachedResponse))
      {
        // we are retrieving the data from the redis cache
        var contentResult = new ContentResult
        {
          Content = cachedResponse,
          ContentType = "application/json",
          StatusCode = 200
        };

        context.Result = contentResult;
        return;
      }

      // if the query is not cached
      var executedContext = await next(); // move to the controller

      if (executedContext.Result is OkObjectResult okObjectResult)
      {
        // put the result into the cache
        await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
      }
    }

    /* 
    method to generate a cache from request object
    this will be used as a key in redis
     */
    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
      var keyBuilder = new StringBuilder();

      keyBuilder.Append($"{request.Path}");

      // loop over the query strings // keys and values in the query
      // because they can come in any order from the client
      foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
      {
        keyBuilder.Append($"|{key}-{value}");
      }

      return keyBuilder.ToString();
    }
  }
}