using System.Threading.Tasks;

namespace API.Interfaces
{
	// Using unit of work to manage repositories
	public interface IUnitOfWork
	{
		IUserRepository UserRepository { get; }
		IMessageRepository MessageRepository { get; }
		ILikesRepository LikesRepository { get; }
		IPhotoRepository PhotoRepository { get; }


		/// <summary>
		/// Save all changes
		/// </summary>
		/// <returns></returns>
		Task<bool> Complete();

		/// <summary>
		/// Check if entity framework has any changes
		/// </summary>
		/// <returns></returns>
		bool HasChanges();
	}
}