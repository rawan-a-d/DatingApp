using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
	/// <summary>
	/// Used in Startup to catch exceptions
	/// </summary>
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;

		/// <summary>
		/// ExceptionMiddleware constructor
		/// </summary>
		/// <param name="next">what's coming next in the middleware</param>
		/// <param name="logger">log the exception in the terminal</param>
		/// <param name="env">environment</param>

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
		{
			_env = env;
			_logger = logger;
			_next = next;
		}
		
		/// <summary>
		/// handles any exception and returns it to the client
		/// </summary>
		/// <param name="context">http context</param>
		/// <returns></returns>
		public async Task InvokeAsync(HttpContext context) {
			try {
				// pass context to the next middleware
				await _next(context);
			}
			catch(Exception ex) {
				// log error to console
				_logger.LogError(ex, ex.Message);
				
				// set response properties
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
				
				// check mode
				var response = _env.IsDevelopment()
					? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
					: new ApiException(context.Response.StatusCode, "Internal Server Error");

				// set json response to camelCase
				var options = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};

				var json = JsonSerializer.Serialize(response, options);

				await context.Response.WriteAsync(json);
			}
		}
	}
}