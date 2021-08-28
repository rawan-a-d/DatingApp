using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	// Seeds the json file into the db
	public class Seed
	{
		/// <summary>
		/// Adds seed data to db, called in Program.cs when the app is started
		/// </summary>
		/// <param name="userManager">userManager from Identity framework, which allows us to get users and update them....</param>
		/// <param name="roleManager">roleManager from Identity framework</param>
		/// <returns></returns>
		public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) {
			// check if users table contains any users
			if(await userManager.Users.AnyAsync()) {
				return;
			}

			// read json file
			var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

			// deserialize JSON into a list of objects
			var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

			// no users
			if(users == null) {
				return;
			}

			// roles
			var roles = new List<AppRole> {
				new AppRole { Name = "Member" },
				new AppRole { Name = "Admin" },
				new AppRole { Name = "Moderator" }
			};
			// create roles
			foreach(var role in roles) {
				await roleManager.CreateAsync(role);
			}


			foreach(var user in users) {
				user.UserName = user.UserName.ToLower();

				// track each user
				await userManager.CreateAsync(user, "Pa$$w0rd"); // it also saves it to db

				// add role to user
				await userManager.AddToRoleAsync(user, "Member");
			}

			// Create an admin
			var admin = new AppUser
			{
				UserName = "admin"
			};
			await userManager.CreateAsync(admin, "Pa$$w0rd");
			await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
		}
	}
}