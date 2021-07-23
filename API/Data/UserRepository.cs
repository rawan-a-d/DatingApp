using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class UserRepository : IUserRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;
		public UserRepository(DataContext context, IMapper mapper)
		{
			_mapper = mapper;
			_context = context;
		}

		/// <summary>
		/// Get a member As MemberDto using AutoMapper queryable extensions -> results in a more effecient query as it directly gets the only fields we need
		/// </summary>
		/// <param name="username">username of the user</param>
		/// <returns>a MemberDto</returns>
		public async Task<MemberDto> GetMemberAsync(string username)
		{
			// Getting fields manually
			//return await _context.Users
			//	.Where(x => x.UserName == username)
			//	.Select(user => new MemberDto
			//	{
			//		Id = user.Id,
			//		Username = user.UserName
			//	}).SingleOrDefaultAsync();

			// Using AutoMapper queryable extensions
			// When using Project, we don't need to include Photos table
			// because Entity framework will work out the correct query
			return await _context.Users
				.Where(x => x.UserName == username)
				.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
				.SingleOrDefaultAsync();
		}

		/// <summary>
		/// Get members list As MemberDto using AutoMapper queryable extensions -> results in a more effecient query as it directly gets the only fields we need
		/// </summary>
		/// <returns>a list of MemberDto</returns>
		public async Task<IEnumerable<MemberDto>> GetMembersAsync()
		{
			return await _context.Users
				.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
				.ToListAsync();	
		}

		public async Task<AppUser> GetUserByIdAsync(int id)
		{
			return await _context.Users.FindAsync(id);
		}

		public async Task<AppUser> GetUserByUsernameAsync(string username)
		{
			return await _context.Users
				.Include(p => p.Photos) // eager loading (include photos of user)
				.SingleOrDefaultAsync(x => x.UserName == username);
		}

		public async Task<IEnumerable<AppUser>> GetUsersAsync()
		{
			return await _context.Users
				.Include(p => p.Photos) // eager loading (include photos of user)
				.ToListAsync();
		}

		public async Task<bool> SaveAllAsync()
		{
			// if changes greater than 0 have been saved -> true
			return await _context.SaveChangesAsync() > 0;
		}

		public void Update(AppUser user)
		{
			// Add flag to the entity that it's been modified
			_context.Entry(user).State = EntityState.Modified;
		}
	}
}