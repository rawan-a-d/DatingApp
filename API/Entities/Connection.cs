namespace API.Entities
{
	/// <summary>
	/// Create a table in the db called Groups
	/// used to track the message groups
	/// </summary>
	public class Connection
	{
		// default constructor, needed for entity framework
		public Connection()
		{
		}

		public Connection(string connectionId, string username)
		{
			ConnectionId = connectionId;
			Username = username;
		}

		// by convention, if we use (name + Id), Entity framework will conisder this the primary key
		public string ConnectionId { get; set; }
		public string Username { get; set; }
	}
}