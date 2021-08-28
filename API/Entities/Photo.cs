using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
	// Create a table in the db called Photots
	// without adding it in the DataContext because we don't need to get individual photos, we need them only in connection with users
	[Table("Photos")]
	public class Photo
	{
		public int Id { get; set; }
		public string Url { get; set; }
		public bool IsMain { get; set; }
		// Cloudinary id, used when deleting
		public string PublicId { get; set; }
		
		// define relationship with user
		// having the list of photos in AppUser is enough
		// but we need this, to change settings on delete and not allow user to be null
		public AppUser AppUser { get; set; }
		public int AppUserId { get; set; }

		// was photo approved by admin
		public bool IsApproved { get; set; } = false;
	}
}