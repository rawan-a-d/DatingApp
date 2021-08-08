using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	/// <summary>
	/// Policy based authorisation
	/// policies are added in IdentityServiceExtensions
	/// </summary>
	public class AdminController : BaseApiController
	{
		private readonly UserManager<AppUser> _userManager;
		public AdminController(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		/// <summary>
		/// Get user with roles
		/// </summary>
		/// <returns></returns>
		[Authorize(Policy = "RequireAdminRole")]
		[HttpGet("users-with-roles")]
		public async Task<ActionResult<ICollection<AppUser>>> GetUsersWithRoles()
		{
			var users = await _userManager.Users
				.Include(r => r.UserRoles)
				.ThenInclude(r => r.Role)
				.OrderBy(u => u.UserName)
				.Select(u => new { // project
					u.Id,
					Username = u.UserName,
					Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
				})
				.ToListAsync();

			return Ok(users);
		}


		/// <summary>
		/// Edit roles of a user
		/// </summary>
		/// <param name="username">username</param>
		/// <param name="roles">the updated roles</param>
		/// <returns></returns>
		//[Authorize(Policy = "")]
		[HttpPost("edit-roles/{username}")]
		public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles) 
		{
			// split roles by comma
			var selectedRoles = roles.Split(",").ToArray();

			// get user by username
			var user = await _userManager.FindByNameAsync(username);

			if(user == null) {
				return NotFound("Could not find user");
			}

			// get user roles
			var userRoles = await _userManager.GetRolesAsync(user);

			// add roles to user unless they are already there
			var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

			if(!result.Succeeded) {
				return BadRequest("Failed to add to roles");
			}

			// remove roles which weren't added
			result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

			if(!result.Succeeded) {
				BadRequest("Failed t remove from roles");
			}

			return Ok(await _userManager.GetRolesAsync(user));
		}


		/// <summary>
		/// Get photos
		/// </summary>
		/// <returns></returns>
		[Authorize(Policy = "ModeratePhotoRole")]
		[HttpGet("photos-to-moderate")]
		public ActionResult GetPhotosForModeration()
		{
			return Ok("Admins or moderators can see this");
		}
	}
}