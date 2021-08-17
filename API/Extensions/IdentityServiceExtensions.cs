using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
			// Configure Identity
			// 1. AddIdentity: used when the server is serving the pages (Session based authorization)
			//services.AddIdentity 
			// 2. AddIdentityCore (basics plus add extras): Single page app (Token based authorization)
			services.AddIdentityCore<AppUser>(opt =>
			{
				opt.Password.RequireNonAlphanumeric = false;
			})
			.AddRoles<AppRole>()
			.AddRoleManager<RoleManager<AppRole>>()
			.AddSignInManager<SignInManager<AppUser>>()
			.AddRoleValidator<RoleValidator<AppRole>>()
			.AddEntityFrameworkStores<DataContext>(); // set up db with all the tables it needs to create dotnet identity tables


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

						// SignalR Authenticating: can't use Authorization header so have to use query strings
						options.Events = new JwtBearerEvents
						{
							OnMessageReceived = context =>
							{
								var accessToken = context.Request.Query["access_token"];

								var path = context.HttpContext.Request.Path;

								// if there is a token and request is sent to hubs/
								if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
								{
									context.Token = accessToken;
								}

								return Task.CompletedTask;
							}
						};
					});

			// Authorization: policy based authorisation
			services.AddAuthorization(opt =>
			{
				opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
				opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
			});

			return services;
		}
	}
}