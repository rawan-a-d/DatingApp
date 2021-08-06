using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			services.AddScoped<IUserRepository, UserRepository>();

			// Likes Repository
			services.AddScoped<ILikesRepository, LikesRepository>();

			// Message Repository
			services.AddScoped<IMessageRepository, MessageRepository>();

			// AutoMapper
			services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

			// make use of created class (DataContext) with the provided connection string
			services.AddDbContext<DataContext>(options => // lambda expression
			{
				// pass connection string to our db (Sqlite)
				// we get connection string from appsettings.Development
				options.UseSqlite(config.GetConnectionString("DefaultConnection"));
			});

			return services;
		}
	}
}