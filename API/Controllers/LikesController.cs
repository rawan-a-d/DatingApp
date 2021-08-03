using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class LikesController : BaseApiController
	{
		private readonly IUserRepository _userRepository;
		private readonly ILikesRepository _likesRepository;
		public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
		{
			_userRepository = userRepository;
			_likesRepository = likesRepository;
		}

		[HttpPost("{username}")] // likes/todd
		public async Task<ActionResult> AddLike(string username) {
			// get current user id
			var sourceUserId = User.GetUserId();

			// get liked user
			var likedUser = await _userRepository.GetUserByUsernameAsync(username);
			// get current user with likes
			var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

			// if liked user is not found
			if(likedUser == null) {
				return NotFound();
			}
			// if user is trying to like himself
			if(likedUser == sourceUser) {
				return BadRequest("You cannot like yourself");
			}

			// if user is already liked
			var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

			if(userLike != null) {
				return BadRequest("You already liked " + username);
			}

			// add user like
			userLike = new UserLike
			{
				SourceUserId = sourceUserId,
				LikedUserId = likedUser.Id,
			};
			sourceUser.LikedUsers.Add(userLike);

			if(await _userRepository.SaveAllAsync()) {
				return Ok();
			}

			return BadRequest("Failed to like user");
		}

		/// <summary>
		/// Get a specific like
		/// </summary>
		/// <param name="sourceUserId">source user id</param>
		/// <param name="likedUserId">liked user id</param>
		/// <returns>a UserLike object with two involved users</returns>
		[HttpGet]
		//public async Task<UserLike> GetUserLike() {
		//	return await _likesRepository.GetUserLike();
		//}

		/// <summary>
		/// Get based on the predicate a list of either the likes the user got or the the likes he made
		/// </summary>
		/// <param name="predicate">condition</param>
		/// <returns>a list of LikeDto</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate) {
			var users = await _likesRepository.GetUserLikes(predicate, User.GetUserId());

			return Ok(users);
		}

		/// <summary>
		/// Get list of users that this user has liked
		/// </summary>
		/// <param name="userId">current user id</param>
		/// <returns>AppUser object which contains the list</returns>
		public async Task<AppUser> GetUserWithLikes(int userId)
		{
			return await _likesRepository.GetUserWithLikes(userId);
		}


	}
}