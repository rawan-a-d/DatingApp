using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

	[ServiceFilter(typeof(LogUserActivity))] // update last active when a user makes a request
	[ApiController]
	[Route("api/[controller]")] // placeholder, gets replaced by first name of control name (WeatherForecast)
	public class BaseApiController : ControllerBase
	{
		
	}
}