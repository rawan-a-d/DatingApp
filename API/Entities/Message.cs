using System;

namespace API.Entities
{
	/// <summary>
	/// Create a table in the db called Messages
	/// defines relationship between Message and AppUser
	/// </summary>
	public class Message
	{
		public int Id { get; set; }
		public int SenderId { get; set; }
		public string SenderUsername { get; set; }
		public AppUser Sender { get; set; }
		public int RecipientId { get; set; }
		public string RecipientUsername { get; set; }
		public AppUser Recipient { get; set; }
		public string Content { get; set; }
		// optional
		public DateTime? DateRead { get; set; }
		//public DateTime MessageSent { get; set; } = DateTime.Now;
		public DateTime MessageSent { get; set; } = DateTime.UtcNow; // same in all browsers

		public bool SenderDeleted { get; set; }
		public bool RecipientDeleted { get; set; }
	}
}