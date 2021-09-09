using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace API.Extensions
{
	public static class ApplicationServiceExtensions
	{
		// Extension method
		// 1. save us from writing repetitive code, we can put it inside of an extension method and reuse it
		// 2. keep startup class as clean as possible
		/// <summary>
		/// Adds TokenService and DataContext to services
		/// </summary>
		/// <param name="services">referes to the same instance of the services in ConfigureServices method in Startup class</param>
		/// <param name="config">configuration</param>
		/// <returns>the services</returns>
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) {
			// CloudinarySettings
			// populate fields in CloudinarySettings class with data in appsettings.json
			services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
			// PhotoService
			services.AddScoped<IPhotoService, PhotoService>();

			// LogUserActivity
			services.AddScoped<LogUserActivity>();

			// Token service
			// 3 ways to do this
			// 1. AddSingleton: doesn't stop until our application stops
			// 2. AddScoped: scoped to the lifetime of the http request
			// 3. AddTransient: destroyed as soon as the method is finished
			services.AddScoped<ITokenService, TokenService>();

			// User Repository
			//services.AddScoped<IUserRepository, UserRepository>();

			// Likes Repository
			//services.AddScoped<ILikesRepository, LikesRepository>();

			// Message Repository
			//services.AddScoped<IMessageRepository, MessageRepository>();

			// Unit of Work
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			// AutoMapper
			services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

			// make use of created class (DataContext) with the provided connection string
			services.AddDbContext<DataContext>(options => // lambda expression
			{
				// pass connection string to our db (Sqlite)
				// we get connection string from appsettings.Development
				//options.UseSqlite(config.GetConnectionString("DefaultConnection"));
				//options.UseNpgsql(config.GetConnectionString("DefaultConnection"));

				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

				string connStr;

				// Depending on if in development or production, use either Heroku-provided
				// connection string, or development connection string from env var.
				if (env == "Development")
				{
					// Use connection string from file.
					connStr = config.GetConnectionString("DefaultConnection");
				}
				else
				{
					// Use connection string provided at runtime by Heroku.
					var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

					// Parse connection URL to connection string for Npgsql
					connUrl = connUrl.Replace("postgres://", string.Empty);
					var pgUserPass = connUrl.Split("@")[0];
					var pgHostPortDb = connUrl.Split("@")[1];
					var pgHostPort = pgHostPortDb.Split("/")[0];
					var pgDb = pgHostPortDb.Split("/")[1];
					var pgUser = pgUserPass.Split(":")[0];
					var pgPass = pgUserPass.Split(":")[1];
					var pgHost = pgHostPort.Split(":")[0];
					var pgPort = pgHostPort.Split(":")[1];

					connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;TrustServerCertificate=True";
				}

				// Whether the connection string came from the local development configuration file
				// or from the environment variable from Heroku, use it to set up your DbContext.
				options.UseNpgsql(connStr);
			});

			// Presence Tracker
			services.AddSingleton<PresenceTracker>();
			
			return services;
		}
	}
}