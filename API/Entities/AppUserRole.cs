using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
	/// <summary>
	/// Defines the relationship between a User and a Role, inherits from IdentityUserRole
	/// </summary>
	public class AppUserRole : IdentityUserRole<int>
	{
		public AppUser User { get; set; }
		public AppRole Role { get; set; }
	}
}