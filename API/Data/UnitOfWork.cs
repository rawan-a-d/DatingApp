using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
	// Manages the creation of repositories objects and passing them the needed data (context and mapper)
	// this will have an instance of the data context, which will be passed to repositories
	public class UnitOfWork : IUnitOfWork
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public UnitOfWork(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public IUserRepository UserRepository => new UserRepository(_context, _mapper);

		public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

		public ILikesRepository LikesRepository => new LikesRepository(_context);

		public IPhotoRepository PhotoRepository => new PhotoRepository(_context, _mapper);

		/// <summary>
		/// Save all changes to db
		/// </summary>
		/// <returns></returns>
		public async Task<bool> Complete()
		{
			return await _context.SaveChangesAsync() > 0;
			
		}

		/// <summary>
		/// Check if entity framework has any changes
		/// </summary>
		/// <returns></returns>
		public bool HasChanges()
		{
			return _context.ChangeTracker.HasChanges();
		}
	}
}