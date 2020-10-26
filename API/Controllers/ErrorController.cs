using API.Errors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  /* 
  we will override the route
  */
  [Route("errors/{code}")]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class ErrorController : BaseController
  {
    public IActionResult Error(int code)
    {
      return new ObjectResult(new ApiResponse(code));
    }
  }
}