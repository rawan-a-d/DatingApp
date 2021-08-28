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
	public class PhotoRepository : IPhotoRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public PhotoRepository(DataContext context, IMapper mapper)
		{
			_mapper = mapper;
			_context = context;
		}

		/// <summary>
		/// Get photo by id
		/// </summary>
		/// <param name="id">the id</param>
		/// <returns>the photo</returns>
		public async Task<Photo> GetPhotoById(int id)
		{
			//var query = _context.Photos
			//	.Where(p => p.Id == id);

			//return await query.FirstOrDefaultAsync();

			return await _context.Photos
				.IgnoreQueryFilters()
				.SingleOrDefaultAsync(x => x.Id == id);
		}

		/// <summary>
		/// Get unapproved photos for all users
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
		{
			return await _context.Photos
				.IgnoreQueryFilters()
				.Where(p => p.IsApproved == false)
				.ProjectTo<PhotoForApprovalDto>(_mapper.ConfigurationProvider)
				.ToListAsync();

			//return await _context.Photos
			//	.IgnoreQueryFilters()
			//	.Where(p => p.IsApproved == false)
			//	.Select(u => new PhotoForApprovalDto
			//	{
			//		Id = u.Id,
			//		Username = u.AppUser.UserName,
			//		Url = u.Url,
			//		IsApproved = u.IsApproved
			//	}).ToListAsync();
		}

		/// <summary>
		/// Remove photo
		/// </summary>
		/// <param name="photo">the photo</param>
		public void RemovePhoto(Photo photo)
		{
			_context.Photos.Remove(photo);
		}
	}
}