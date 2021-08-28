using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	// [ApiController]
	// [Route("api/[controller]")] // api/Users
	// Because of the inheritance, we no longer need the attributes, methods and properties which are in BaseApiController
	[Authorize]
	public class UsersController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;

		public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
		{
			_photoService = photoService;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}


		/// <summary>
		/// Get all users as a IEnumerable Asynchronous
		/// Can also use a regular List
		/// IEnumerable allows us to use simple iteration over a collection of specified type
		/// whereas List offers methods to search, sort, manipulate which we don't need
		/// </summary>
		/// <param name="userParams">query stirng (page number and page size)</param>
		/// <returns> a list of users </returns>
		///
		//[AllowAnonymous]
		//[Authorize(Roles = "Admin")]
		[HttpGet] // api/users
		public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
		{
			// get gender
			var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());

			userParams.CurrentUsename = User.GetUsername();

			// if no gender is selected, use opposite gender
			if(string.IsNullOrEmpty(userParams.Gender)) {
				userParams.Gender = gender == "male" ? "female" : "male";
			}

			// get all users using query parameters
			var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

			// add pagination header to response
			Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

			return Ok(users);
		}

		/// <summary>
		/// Get a user with specified id Asynchronous
		/// </summary>
		/// <param name="id"></param>
		/// <returns> a user with specified id </returns>
		///
		//[Authorize] 
		//[Authorize(Roles = "Member")]
		[HttpGet("{username}", Name = "GetUser")] // api/users/lisa
		public async Task<ActionResult<MemberDto>> GetUser(string username)
		{
			var isCurrentUser = username == User.GetUsername();

			var user = await _unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser);

			return user;
		}


		/// <summary>
		/// Update user profile
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		[HttpPut]
		public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
		{
			// get username from token
			var username = User.GetUsername();

			// get user by username
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

			// map from MemberUpdateDto to AppUser
			_mapper.Map(memberUpdateDto, user);

			// flag user as changed
			_unitOfWork.UserRepository.Update(user);

			// push changes to db
			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user");
		}


		[HttpPost("add-photo")]
		public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
		{
			// get username from token
			var username = User.GetUsername();

			// get user object with the photos
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

			// add new photo to Cloudinary
			var result = await _photoService.AddPhotoAsync(file);

			// if there was an error
			if(result.Error != null) {
				return BadRequest(result.Error.Message);
			}

			// otherwise
			// Create new photo object using the result
			var photo = new Photo
			{
				Url = result.SecureUrl.AbsoluteUri,
				PublicId = result.PublicId
			};

			// if user has no photos
			//if(user.Photos.Count == 0) {
			//	photo.IsMain = true;
			//}

			// add photo to photos array
			user.Photos.Add(photo);

			// save changes to db
			if(await _unitOfWork.Complete()) {
				// created 201 response and location header for the user (https://localhost:5001/api/Users/lisa)
				return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
			}

			return BadRequest("Problem adding photos");
		}

		/// <summary>
		/// Set selected photo as main
		/// </summary>
		/// <param name="photoId">selected photo id</param>
		/// <returns>no content response</returns>
		[HttpPut("set-main-photo/{photoId}")]
		public async Task<ActionResult> SetMainPhoto(int photoId) 
		{
			// get user
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			// find photo
			var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

			// photo is already main
			if(photo.IsMain) {
				return BadRequest("This is already your main photo");
			}
			// if photo is not approved
			if(photo.IsApproved == false) {
				return BadRequest("This photo is not approved");
			}

			// remove currentMain
			//var currentMain = user.Photos.Where(x => x.IsMain).ToList();
			var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

			if(currentMain != null) {
				currentMain.IsMain = false;
				//for (int i = 0; i < currentMain.Count; i++)
				//{
				//	if(user.Photos.FirstOrDefault(x => x.Id == currentMain[i].Id).IsMain) {
				//		//return BadRequest("This is already your main photo");
				//		currentMain[i].IsMain = false;
				//	}
				//}
			}

			// set photo to main
			photo.IsMain = true;

			// save to db
			if(await _unitOfWork.Complete()) {
				return NoContent();
			}

			return BadRequest("Failed to set main photo");
		}

		/// <summary>
		/// Delete a photo
		/// </summary>
		/// <param name="photoId">photo id</param>
		/// <returns>nothing</returns>
		[HttpDelete("delete-photo/{photoId}")]
		public async Task<ActionResult> DeletePhoto(int photoId) 
		{
			// get user
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			// find photo
			var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

			if(photo == null) {
				return NotFound();
			}

			// remove photo
			// 1) allow deletion of main
			//// 1. from Cloudinary
			//if(photo.PublicId != null) {
			//	// delete photo from Cloudinary
			//	var result = await _photoService.DeletePhotoAsync(photo.PublicId);

			//	if(result.Error != null) {
			//		return BadRequest(result.Error.Message);
			//	}
			//}
			//// 2. from DB
			//user.Photos.Remove(photo);

			//// photo is main
			//if(photo.IsMain) {
			//	// set first photo to main
			//	user.Photos.First().IsMain = true;
			//}


			// 2) don't allow deletion of main
			// photo is main
			if(photo.IsMain) {
				return BadRequest("You cannout delete your main photo");
			}

			// remove photo
			// 1. from Cloudinary
			if(photo.PublicId != null) {
				// delete photo from Cloudinary
				var result = await _photoService.DeletePhotoAsync(photo.PublicId);

				if(result.Error != null) {
					return BadRequest(result.Error.Message);
				}
			}
			// 2. from DB
			user.Photos.Remove(photo);


			// save to db
			if(await _unitOfWork.Complete()) {
				return Ok();
			}

			return BadRequest("Failed to delete photo");
		}
	}
}