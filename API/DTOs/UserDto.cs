namespace API.DTOs
{
	/// <summary>
	/// Returned when the user logs in or registers
	/// </summary>
	public class UserDto
	{
		public string Username { get; set; }
		
		public string Token { get; set; }
		// main photo
		public string PhotoUrl { get; set; }

		public string KnownAs { get; set; }

		// easier to use, when getting all members
		public string Gender { get; set; }
	}
}