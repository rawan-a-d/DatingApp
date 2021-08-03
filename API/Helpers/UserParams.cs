namespace API.Helpers
{
	// User query params received with the http request
	// pagination, selection of gender and excluding of current user
	public class UserParams: PaginationParams
	{
		// used to exclude current user from list
		public string CurrentUsename { get; set; }

		// selected gender
		public string Gender { get; set; }

		// age
		public int MinAge { get; set; } = 18;
		public int MaxAge { get; set; } = 150;

		// sorting
		// last active or created at
		public string OrderBy { get; set; } = "lastActive";
	}
}