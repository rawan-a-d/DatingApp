namespace API.Entities
{
	public class AppUser
	{
		// generate by prop -> tab
		public int Id { get; set; }
		public string UserName { get; set; }
		
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }

		// generate by propfull -> tab
		// public int MyProperty
		// {
		//     get { return myVar; }
		//     set { myVar = value; }
		// }

	}
}