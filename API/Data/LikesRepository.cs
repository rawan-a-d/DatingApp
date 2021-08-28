using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	/// <summary>
	/// Handles db transactions with the Likes table
	/// </summary>
	public class LikesRepository : ILikesRepository
	{
		private readonly DataContext _context;
		
		public LikesRepository(DataContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Get a specific like
		/// </summary>
		/// <param name="sourceUserId">source user id</param>
		/// <param name="likedUserId">liked user id</param>
		/// <returns>a UserLike object with two involved users</returns>
		public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
		{
			return await _context.Likes
				.FindAsync(sourceUserId, likedUserId);
		}

		/// <summary>
		/// Get based on the predicate a list of either the likes the user got or the the likes he made
		/// </summary>
		/// <param name="predicate">condition</param>
		/// <param name="userId">user id</param>
		/// <returns>a list of LikeDto</returns>
		public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
		{
			// users query ordered by username
			var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();

			// likes query
			var likes = _context.Likes.AsQueryable();

			// users the current user has liked
			if(likesParams.Predicate == "liked") {
				likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
				// users from liked table
				users = likes.Select(like => like.LikedUser);
			}
			// users who liked the current user
			else if(likesParams.Predicate == "likedBy") {
				likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
				// users from liked table
				users = likes.Select(like => like.SourceUser);            
			}
			// if no predicate
			else {
				return null;
			}

			// project manually instead of using Mapper
			var query = users.Select(user => new LikeDto
			{
				Username = user.UserName,
				KnownAs = user.KnownAs,
				Age = user.DateOfBirth.CalculateAge(),
				PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
				City = user.City,
				Id = user.Id
			});

			return await PagedList<LikeDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
		}

		/// <summary>
		/// Get list of users that this user has liked
		/// </summary>
		/// <param name="userId">current user id</param>
		/// <returns>AppUser object which contains the list</returns>
		public async Task<AppUser> GetUserWithLikes(int userId)
		{
			return await _context.Users
				.Include(x => x.LikedUsers) // include liked users
				.FirstOrDefaultAsync(x => x.Id == userId);
		}
	}
}