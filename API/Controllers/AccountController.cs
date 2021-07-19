using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly DataContext _context;
		private readonly ITokenService _tokenService;

		public AccountController(DataContext context, ITokenService tokenService)
		{
			_context = context;
			_tokenService = tokenService;
		}

		/// <summary>
		/// Register a new user
		/// </summary>
		/// <param name="registerDto">user credentials (username and password)</param>
		/// <returns>UserDto object with the user name and token</returns>
		/// 
		[HttpPost("register")]
		//public async Task<ActionResult<AppUser>> Register(string username, string password) {
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			// Hashing algorithm
			using var hmac = new HMACSHA512();

			// If username is taken
			if (await UserExists(registerDto.Username))
			{
				return BadRequest("Username is taken");
			}

			// new user
			var user = new AppUser
			{
				UserName = registerDto.Username.ToLower(),
				PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
				PasswordSalt = hmac.Key // save the key to use in the future in the hasing algorithm (for login)
			};

			// Track new user
			_context.Users.Add(user);
			// Push user to db
			await _context.SaveChangesAsync();

			return new UserDto
			{
				Username = user.UserName,
				Token = _tokenService.CreateToken(user)
			};
		}

		/// <summary>
		/// Login endpoint
		/// </summary>
		/// <param name="loginDto">user credentials (username and password)</param>
		/// <returns>UserDto object with the user name and token</returns>
		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await _context.Users
								.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

			if (user == null)
			{
				return Unauthorized("Invalid username");
			}

			// Validate password
			// 1. hashing algorithm using the same key as when the user registered
			using var hmac = new HMACSHA512(user.PasswordSalt);

			// 2. hash the password provided
			var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

			// 3. compare provided hashed password to hashed password in the db
			for (int i = 0; i < computedHash.Length; i++) {
				// 4. if password is not correct -> Unauthorized
				if(computedHash[i] != user.PasswordHash[i]) {
					return Unauthorized("Invalid password");
				}
			}

			return new UserDto
			{
				Username = user.UserName,
				Token = _tokenService.CreateToken(user)
			};
		}
	

		/// <summary>
		/// Check if username is already taken
		/// </summary>
		/// <param name="username">chosen username</param>
		/// <returns>true or false</returns>
		private async Task<bool> UserExists(string username) {
				return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
			}
		}
}