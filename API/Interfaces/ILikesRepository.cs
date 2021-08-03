using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        // Get a specific like
		Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);

        // Get a user with his likes
		Task<AppUser> GetUserWithLikes(int userId);

        // Get based on the predicate a list of either the likes the user got or the the likes he made 
		Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
	}
}