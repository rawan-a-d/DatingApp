namespace API.DTOs
{
	// Used to create a new message
	public class CreateMessageDto
	{
		public string RecipientUsername { get; set; }
		public string Content { get; set; }
	}
}