namespace API.Helpers
{
	public class MessageParams: PaginationParams
	{
		// current user
		public string Username { get; set; }
		public string Container { get; set; } = "Unread";
	}
}