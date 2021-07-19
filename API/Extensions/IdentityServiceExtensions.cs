using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
		// Extension method
		// 1. save us from writing repetitive code, we can put it inside of an extension method and reuse it
		// 2. keep startup class as clean as possible
		public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config) {
			// Authenticate using Bearer
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						// validate token using TokenKey
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
							ValidateIssuer = false, // API server
							ValidateAudience = false, // Angular client
						};
					});

			return services;
		}
    }
}