using System.Collections.Generic;
using System.Linq;
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
			var users = await _userRepository.GetUsersAsync();

			// map list of users to a list of MemberDto
			var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

			return Ok(usersToReturn);
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
			var user = await _userRepository.GetUserByUsernameAsync(username);

			return _mapper.Map<MemberDto>(user);
		}


		/// <summary>
		/// Get all users as a IEnumerable
		/// Can also use a regular List
		/// IEnumerable allows us to use simple iteration over a collection of specified type
		/// whereas List offers methods to search, sort, manipulate which we don't need
		/// </summary>
		/// <returns> a list of users </returns>
		///
		/* [HttpGet] // api/users
		public ActionResult<IEnumerable<AppUser>> GetUsers() 
		{
			var users = _context.Users.ToList();

			return users;
		} */


		/// <summary>
		/// Get a user with specified id
		/// </summary>
		/// <param name="id"></param>
		/// <returns> a user with specified id </returns>
		///
		/* [HttpGet("{id}")] // api/users/2
		public ActionResult<AppUser> GetUser(int id)
		{
			var user = _context.Users.Find(id);

			return user;
		} */
	}
}