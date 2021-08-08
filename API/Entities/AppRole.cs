using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
	/// <summary>
	/// Defines the role of a user, inherits from IdentityRole
	/// </summary>
	public class AppRole : IdentityRole<int>
	{
		public ICollection<AppUserRole> UserRoles { get; set; }
	}
}