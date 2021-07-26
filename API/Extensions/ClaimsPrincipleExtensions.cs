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
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}