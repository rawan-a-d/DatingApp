using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
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
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPhotoService _photoService;
		public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
		{
			_photoService = photoService;
			_unitOfWork = unitOfWork;
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
				.Select(u => new
				{ // project
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

			if (user == null)
			{
				return NotFound("Could not find user");
			}

			// get user roles
			var userRoles = await _userManager.GetRolesAsync(user);

			// add roles to user unless they are already there
			var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

			if (!result.Succeeded)
			{
				return BadRequest("Failed to add to roles");
			}

			// remove roles which weren't added
			result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

			if (!result.Succeeded)
			{
				BadRequest("Failed t remove from roles");
			}

			return Ok(await _userManager.GetRolesAsync(user));
		}


		/// <summary>
		/// Get photos for approval
		/// </summary>
		/// <returns>list of unapproved photos</returns>
		[Authorize(Policy = "ModeratePhotoRole")]
		[HttpGet("photos-to-moderate")]
		public async Task<ActionResult<IEnumerable<PhotoForApprovalDto>>> GetPhotosForModeration()
		{
			var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();

			return Ok(photos);
		}


		/// <summary>
		/// Approve photo by id
		/// </summary>
		/// <returns></returns>
		[Authorize(Policy = "ModeratePhotoRole")]
		[HttpPost("approve-photo/{photoId}")]
		public async Task<ActionResult> ApprovePhoto(int photoId)
		{
			var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

			if (photo == null)
			{
				return NotFound("Photo was not found");
			}

			photo.IsApproved = true;

			// get user
			var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);

			// if user has no main photo
			if (string.IsNullOrEmpty(user.PhotoUrl))
			{
				photo.IsMain = true;
			}
			//if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

			if (await _unitOfWork.Complete())
			{
				return Ok();
			}

			return BadRequest("Photo cannot be approved");
		}


		/// <summary>
		/// Reject photo by id
		/// </summary>
		/// <returns></returns>
		[Authorize(Policy = "ModeratePhotoRole")]
		[HttpPost("reject-photo/{photoId}")]
		public async Task<ActionResult> RejectPhoto(int photoId)
		{
			var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

			if (photo == null) return NotFound("Could not find photo");


			//_unitOfWork.PhotoRepository.RemovePhoto(photoId);

			// if photo ha public id from Cloudinary
			if (photo.PublicId != null)
			{
				// delete photo from Cloudinary
				var result = await _photoService.DeletePhotoAsync(photo.PublicId);

				if (result.Result == "ok") {
					_unitOfWork.PhotoRepository.RemovePhoto(photo);
				}
			}
			else
			{
				_unitOfWork.PhotoRepository.RemovePhoto(photo);
			}

			if (await _unitOfWork.Complete())
			{
				return Ok();
			}

			return BadRequest("Photo cannot be removed");
		}
	}
}