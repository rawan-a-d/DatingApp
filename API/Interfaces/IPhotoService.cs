using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.Interfaces
{
	/// <summary>
	/// Interface for uploading and deleting photos from Cloudinary
	/// </summary>
	public interface IPhotoService
	{
		Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
		Task<DeletionResult> DeletePhotoAsync(string publicId);  
	}
}