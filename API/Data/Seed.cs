using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	// Seeds the json file into the db
	public class Seed
	{
		/// <summary>
		/// Adds seed data to db, called in Program.cs when the app is started
		/// </summary>
		/// <param name="context">db context</param>
		/// <returns>void</returns>
		public static async Task SeedUsers(DataContext context) {
			// check if users table contains any users
			if(await context.Users.AnyAsync()) {
				return;
			}

			// read json file
			var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

			// deserialize JSON into a list of objects
			var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

			foreach(var user in users) {
				using var hmac = new HMACSHA512();

				user.UserName = user.UserName.ToLower();
				user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
				user.PasswordSalt = hmac.Key;

				// track each user
				context.Users.Add(user);
			}

			// save users to db
			await context.SaveChangesAsync();
		}
	}
}