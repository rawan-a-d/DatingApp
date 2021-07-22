using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
	/// <summary>
	/// Class with type ApiController
	/// Route: how is the user going to GET to the api controller from the client
	/// Derives from ControllerBase which is a base class for MVC controller without view support
	/// </summary>
	[ApiController]
	[Route("[controller]")] // placeholder, gets replaced by first name of control name (WeatherForecast)
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		// Get request
		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
			var rng = new Random();
			
			// return list of weather forecast
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = rng.Next(-20, 55),
				Summary = Summaries[rng.Next(Summaries.Length)]
			})
			.ToArray();
		}
	}
}
