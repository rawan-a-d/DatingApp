using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

	[ApiController]
	[Route("api/[controller]")] // placeholder, gets replaced by first name of control name (WeatherForecast)
	public class BaseApiController : ControllerBase
    {
        
    }
}