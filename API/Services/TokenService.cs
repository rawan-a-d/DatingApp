using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
	public class TokenService : ITokenService
	{
		// Symmetric Encryption: one key is use to encrypt and decrypt information
		// Asymmetric encryption uses: two keys (public and private) to encrypt and decryt info, this is how HTTPS work
		private readonly SymmetricSecurityKey _key;
		private readonly UserManager<AppUser> _userManager;

		public TokenService(IConfiguration config, UserManager<AppUser> userManager)
		{
			_userManager = userManager;
			_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
		}


		/// <summary>
		/// Creates a token for user
		/// </summary>
		/// <param name="user">the AppUser object</param>
		/// <returns>the created token</returns>
		public async Task<string> CreateToken(AppUser user)
		{
			// 1. add claims
			var claims = new List<Claim> {
				new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
				//new Claim(JwtRegisteredClaimNames.R)
			};

			// get user roles
			var roles = await _userManager.GetRolesAsync(user);

			// add roles to token
			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			// 2. create credentials
			// takes in TokenKey and Security algorithm
			var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

			// 3. describe how the token is going to look
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(7),
				SigningCredentials = creds
			};

			// 4. create token handler
			var tokenHandler = new JwtSecurityTokenHandler();

			// 5. create token
			var token = tokenHandler.CreateToken(tokenDescriptor);

			// 6. return written token as string
			return tokenHandler.WriteToken(token);
		}
	}
}