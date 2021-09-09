using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API
{
	public class Startup
	{
		// public Startup(IConfiguration configuration)
		// {
		//     Configuration = configuration;
		// }

		//public IConfiguration Configuration { get; }
		
		/// <summary>
		/// Configuration file data
		/// </summary>
		private readonly IConfiguration _config;

		public Startup(IConfiguration config)
		{
			_config = config;
		}


		// This method gets called by the runtime. Use this method to add services to the container.
		// Dependency Injection Container
		// Order does not matter
		public void ConfigureServices(IServiceCollection services)
		{
			// Extension method
			// code inside this method was HERE
			services.AddApplicationServices(_config);

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
			});

			// CORS (Cross Origin Requests) support
			services.AddCors();

			// Extension method
			// code inside this method was HERE
			services.AddIdentityServices(_config);

			// SignalR
			services.AddSignalR();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		// Ordering matters
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			// check if we're in development mode
			// Middleware to catch exceptions if there's no error handling in any of the bottom classes/methods
			// if (env.IsDevelopment())
			// {
			// 	app.UseDeveloperExceptionPage();
			// 	app.UseSwagger();
			// 	app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
			// }

			// Middleware to catch exceptions
			app.UseMiddleware<ExceptionMiddleware>();

			// redirect from HTTP address to the HTTP end-point
			app.UseHttpsRedirection();

			app.UseRouting();

			// CORS (Cross Origin Requests) support
			app.UseCors(policy => policy.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials() // for SignalR
			.WithOrigins("https://localhost:4200"));

			// authenticate that the user has a jwt when there's an Authenticate above the route
			app.UseAuthentication();
			app.UseAuthorization();

			// files to be served
			// if index.html is found, use it
			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();

				// SignalR
				endpoints.MapHub<PresenceHub>("hubs/presence");
				endpoints.MapHub<MessageHub>("hubs/message");

				// Fallback controller to handle Angular routing
				endpoints.MapFallbackToController("Index", "Fallback");
			});
		}
	}
}
