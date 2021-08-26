using System;
using System.Text.Json.Serialization;

namespace API.DTOs
{
	public class MessageDto
	{
		public int Id { get; set; }
		public int SenderId { get; set; }
		public string SenderUsername { get; set; }
		public string SenderPhotoUrl { get; set; }
		public int RecipientId { get; set; }
		public string RecipientUsername { get; set; }
		public string RecipientPhotoUrl { get; set; }
		public string Content { get; set; }
		// optional
		public DateTime? DateRead { get; set; }
		public DateTime MessageSent { get; set; }

		// we don't want to send these back (used in the query only)
		[JsonIgnore]
		public bool SenderDeleted { get; set; }
		[JsonIgnore]
		public bool RecipientDeleted { get; set; }
	}
}