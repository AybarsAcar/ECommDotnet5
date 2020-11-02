using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  /* 
  serves the compiled Angular project
  the view layer of the application
   */
  public class FallbackController : Controller
  {
    public IActionResult Index()
    {
      return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
    }
  }
}