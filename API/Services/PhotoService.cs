using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
	public class PhotoService : IPhotoService
	{
		private readonly Cloudinary _cloudinary;
		public PhotoService(IOptions<CloudinarySettings> config)
		{
			// Cloudinary account
			var acc = new Account(
				config.Value.CloudName,
				config.Value.ApiKey,
				config.Value.ApiSecret
			);

			// Initialize _cloudinary object
			_cloudinary = new Cloudinary(acc);
		}

		/// <summary>
		/// Uploads an images to cloudinary
		/// </summary>
		/// <param name="file">the new photo</param>
		/// <returns>ImageUploadResult which contains information about the newly created image</returns>
		public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
		{
			var uploadResult = new ImageUploadResult();

			// if file is not empty
			if(file.Length > 0) {
				// Upload file
				// 1. open read stream
				using var stream = file.OpenReadStream();
				// 2. set upload params
				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(file.FileName, stream),
					// make image square and focus on face
					Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
				};
				// 3. upload file
				uploadResult = await _cloudinary.UploadAsync(uploadParams);
			}

			return uploadResult;
		}

		/// <summary>
		/// Delete photo from Cloudinary
		/// </summary>
		/// <param name="publicId">id of photo</param>
		/// <returns>result of deletion</returns>
		public async Task<DeletionResult> DeletePhotoAsync(string publicId)
		{
			var deleteParams = new DeletionParams(publicId);

			var result = await _cloudinary.DestroyAsync(deleteParams);

			return result;
		}
	}
}