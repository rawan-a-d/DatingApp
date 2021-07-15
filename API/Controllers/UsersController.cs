using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")] // api/Users
	public class UsersController : ControllerBase
	{
		private readonly DataContext _context;
		public UsersController(DataContext context)
		{
			_context = context;
		}
		

		/// <summary>
		/// Get all users as a IEnumerable Asynchronous
		/// Can also use a regular List
		/// IEnumerable allows us to use simple iteration over a collection of specified type
		/// whereas List offers methods to search, sort, manipulate which we don't need
		/// </summary>
		/// <returns> a list of users </returns>
		///
		[HttpGet] // api/users
		public async Task <ActionResult<IEnumerable<AppUser>>> GetUsers()
		{
			return await _context.Users.ToListAsync();
		}

		/// <summary>
		/// Get a user with specified id Asynchronous
		/// </summary>
		/// <param name="id"></param>
		/// <returns> a user with specified id </returns>
		///
		[HttpGet("{id}")] // api/users/2
		public async Task<ActionResult<AppUser>> GetUser(int id)
		{
			return await _context.Users.FindAsync(id);
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