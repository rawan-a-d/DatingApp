using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
		{
			_mapper = mapper;
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
		}

		/// <summary>
		/// Register a new user
		/// </summary>
		/// <param name="registerDto">user credentials (username and password)</param>
		/// <returns>UserDto object with the user name and token</returns>
		/// 
		[HttpPost("register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			// If username is taken
			if (await UserExists(registerDto.Username))
			{
				return BadRequest("Username is taken");
			}

			// Map registerDto to AppUser
			var user = _mapper.Map<AppUser>(registerDto);

			// Set username and passwords
			user.UserName = registerDto.Username.ToLower();

			// Track new user
			var result = await _userManager.CreateAsync(user, registerDto.Password);

			if(!result.Succeeded) {
				return BadRequest(result.Errors);
			}

			// Add role to user (member)
			var roleResult = await _userManager.AddToRoleAsync(user, "Member");

			if(!roleResult.Succeeded) {
				return BadRequest(result.Errors);
			}


			return new UserDto
			{
				Username = user.UserName,
				Token = await _tokenService.CreateToken(user),
				KnownAs = user.KnownAs,
				Gender = user.Gender
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
			var user = await _userManager.Users
							.Include(p => p.Photos)
							.SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

			if (user == null)
			{
				return Unauthorized("Invalid username");
			}

			// Sign in user
			// takes three paramaters (user object, password, lock out user if password is wrong)
			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

			// if not success
			if(!result.Succeeded) {
				return Unauthorized();
			}


			return new UserDto
			{
				Username = user.UserName,
				Token = await _tokenService.CreateToken(user),
				PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
				KnownAs = user.KnownAs,
				Gender = user.Gender
			};
		}


		/// <summary>
		/// Check if username is already taken
		/// </summary>
		/// <param name="username">chosen username</param>
		/// <returns>true or false</returns>
		private async Task<bool> UserExists(string username)
		{
			return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
		}
	}
}