using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class UserRepository : IUserRepository
	{
		private readonly DataContext _context;
		public UserRepository(DataContext context)
		{
			_context = context;
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