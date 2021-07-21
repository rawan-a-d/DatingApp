using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
	public class TokenService : ITokenService
	{
		// Symmetric Encryption: one key is use to encrypt and decrypt information
		// Asymmetric encryption uses: two keys (public and private) to encrypt and decryt info, this is how HTTPS work
		private readonly SymmetricSecurityKey _key;

		public TokenService(IConfiguration config)
		{
			_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
		}


		/// <summary>
		/// Creates a token for user
		/// </summary>
		/// <param name="user">the AppUser object</param>
		/// <returns>the created token</returns>
		public string CreateToken(AppUser user)
		{
			// 1. add claims
			var claims = new List<Claim> {
				new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
			};

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