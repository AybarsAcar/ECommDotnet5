using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class BuggyController : BaseController
  {
    private readonly StoreContext _context;
    public BuggyController(StoreContext context)
    {
      this._context = context;
    }

    [HttpGet("testauth")]
    [Authorize]
    public ActionResult<string> GetSecretText()
    {
      return "secret message";
    }

    [HttpGet("notfound")]
    public ActionResult GetNotFound()
    {
      var thing = _context.Products.Find(-1);

      if (thing == null)
      {
        return NotFound(new ApiResponse(404));
      }

      return Ok();
    }

    [HttpGet("servererror")]
    public ActionResult GetServerError()
    {
      var thing = _context.Products.Find(-1);

      var thingToReturn = thing.Id;

      return Ok();
    }

    [HttpGet("badrequest")]
    public ActionResult GetBadRequest()
    {
      return BadRequest(new ApiResponse(400));
    }

    [HttpGet("badrequest/{id}")]
    public ActionResult GetNotFoundRequest(int id)
    {
      return Ok();
    }
  }
}