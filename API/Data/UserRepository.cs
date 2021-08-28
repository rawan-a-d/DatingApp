using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
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
		public async Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser)
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
			//return await _context.Users
			//	.Where(x => x.UserName == username)
			//	.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
			//	.SingleOrDefaultAsync();

			// get member with their approved photos only
			var query = _context.Users
				.Where(x => x.UserName == username)
				.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
				.AsQueryable();

			// Ignore Query filter for the current user so the current user still sees their unapproved photos
			if(isCurrentUser) {
				query = query.IgnoreQueryFilters();
			}

			return await query.FirstOrDefaultAsync();
		}

		/// <summary>
		/// Get members list As MemberDto using AutoMapper queryable extensions -> results in a more effecient query as it directly gets the only fields we need
		/// Paginate it based on the the params the user sent with the request
		/// </summary>
		/// <returns>a list of MemberDto</returns>
		public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
		{
			// 1. create query
			var query = _context.Users.AsQueryable();

			// 2. filter by username and gender
			query = query.Where(u => u.UserName != userParams.CurrentUsename);
			query = query.Where(u => u.Gender == userParams.Gender);

			// max age = 30, min age = 20
			var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1); // -29
			var maxDob = DateTime.Today.AddYears(-userParams.MinAge); // -20

			query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

			// sorting
			query = userParams.OrderBy switch
			{
				"created" => query.OrderByDescending(u => u.Created),
				_ => query.OrderByDescending(u => u.LastActive) // default
			};

			// 3. project
			var projectedQuery = query
				.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
				.AsNoTracking(); // because we're only reading this (can be removed)

			// 4. add pagination and execute query
			return await PagedList<MemberDto>.CreateAsync(projectedQuery, userParams.PageNumber, userParams.PageSize);
		}

		/// <summary>
		/// Get user by id
		/// </summary>
		/// <param name="id">the id</param>
		/// <returns>the user as AppUser</returns>
		public async Task<AppUser> GetUserByIdAsync(int id)
		{
			return await _context.Users.FindAsync(id);
		}

		/// <summary>
		/// Get user by photo id
		/// </summary>
		/// <param name="photoId">the photo id</param>
		/// <returns>the user</returns>
		public async Task<MemberDto> GetUserByPhotoId(int photoId)
		{
			return await _context.Users
				.Include(p => p.Photos)
				.IgnoreQueryFilters()
				.Where(p => p.Photos.Any(p => p.Id == photoId))
				.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();
		}

		/// <summary>
		/// Get user by username
		/// </summary>
		/// <param name="username">the username</param>
		/// <returns>the user as AppUser</returns>
		public async Task<AppUser> GetUserByUsernameAsync(string username)
		{
			return await _context.Users
				.Include(p => p.Photos) // eager loading (include photos of user)
				.IgnoreQueryFilters() // include photos which are not approved
				.SingleOrDefaultAsync(x => x.UserName == username);
		}

		/// <summary>
		/// Get user gender
		/// </summary>
		/// <param name="username">the username</param>
		/// <returns>the gender</returns>
		public async Task<string> GetUserGender(string username)
		{
			return await _context.Users.Where(x => x.UserName == username)
				.Select(x => x.Gender)
				.FirstOrDefaultAsync();
		}

		/// <summary>
		/// Get users
		/// </summary>
		/// <returns>a list of users</returns>
		public async Task<IEnumerable<AppUser>> GetUsersAsync()
		{
			return await _context.Users
				.Include(p => p.Photos) // eager loading (include photos of user)
				.ToListAsync();
		}

		/// <summary>
		/// Update a user
		/// </summary>
		/// <param name="user">the user</param>
		public void Update(AppUser user)
		{
			// Add flag to the entity that it's been modified
			_context.Entry(user).State = EntityState.Modified;
		}
	}
}