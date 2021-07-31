using System.Security.Claims;

namespace API.Extensions
{
	public static class ClaimsPrincipleExtensions
	{
		/// <summary>
		/// Get username from token
		/// </summary>
		/// <param name="user"></param>
		/// <returns>username</returns>
		public static string GetUsername(this ClaimsPrincipal user) {
			return user.FindFirst(ClaimTypes.Name)?.Value;
		}

		/// <summary>
		/// get id from token
		/// </summary>
		/// <param name="user"></param>
		/// <returns>id</returns>
		public static int GetUserId(this ClaimsPrincipal user) {
			return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
		}
	}
}