using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Handles Angular routes
    // if startup doesn't know route fall back to this controller
    public class FallbackController : Controller
    {
        // direct to index in the angular app which can handle the routing
        public ActionResult Index() {
			return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
		}
    }
}