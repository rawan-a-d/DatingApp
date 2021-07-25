using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	// [ApiController]
	// [Route("api/[controller]")] // api/Users
	// Because of the inheritance, we no longer need the attributes, methods and properties which are in BaseApiController
	[Authorize]
	public class UsersController : BaseApiController
	{
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		public UsersController(IUserRepository userRepository, IMapper mapper)
		{
			_mapper = mapper;
			_userRepository = userRepository;
		}


		/// <summary>
		/// Get all users as a IEnumerable Asynchronous
		/// Can also use a regular List
		/// IEnumerable allows us to use simple iteration over a collection of specified type
		/// whereas List offers methods to search, sort, manipulate which we don't need
		/// </summary>
		/// <returns> a list of users </returns>
		///
		//[AllowAnonymous]
		[HttpGet] // api/users
		public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
		{
			//return await _context.Users.ToListAsync();
			var users = await _userRepository.GetMembersAsync();

			// map list of users to a list of MemberDto
			//var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

			return Ok(users);
		}

		/// <summary>
		/// Get a user with specified id Asynchronous
		/// </summary>
		/// <param name="id"></param>
		/// <returns> a user with specified id </returns>
		///
		//[Authorize] 
		[HttpGet("{username}")] // api/users/lisa
		public async Task<ActionResult<MemberDto>> GetUser(string username)
		{
			var user = await _userRepository.GetMemberAsync(username);

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
			var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			// get user by username
			var user = await _userRepository.GetUserByUsernameAsync(username);

			// map from MemberUpdateDto to AppUser
			_mapper.Map(memberUpdateDto, user);

			// flag user as changed
			_userRepository.Update(user);

			// push changes to db
			if(await _userRepository.SaveAllAsync()) {
				return NoContent();
			}
			return BadRequest("Failed to update user");
		}
	} 
}