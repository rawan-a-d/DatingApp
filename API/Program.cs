using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
	public class Program
	{
		// First method to run
		public static async Task Main(string[] args)
		{
			//CreateHostBuilder(args).Build().Run();
			var host = CreateHostBuilder(args).Build();

			// Add data to DB
			using var scope = host.Services.CreateScope();
			var services = scope.ServiceProvider;

			try {
				var context = services.GetRequiredService<DataContext>();
				// applies pending migrations to the db, creates db if does not already exist
				await context.Database.MigrateAsync();

				// add seed users to db
				await Seed.SeedUsers(context);
			}
			catch(Exception ex) {
				var logger = services.GetRequiredService<ILogger<Program>>();
				logger.LogError(ex, "An error occured during migration");
			}

			await host.RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					// Use Startup class
					webBuilder.UseStartup<Startup>();
				});
	}
}
