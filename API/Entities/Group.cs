using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
	/// <summary>
	/// Create a table in the db called Groups
	/// used to track the message groups
	/// </summary>
	public class Group
	{
		public Group()
		{
		}

		public Group(string name)
		{
			Name = name;
		}

		// Primary key
		[Key]
		public string Name { get; set; }
		public ICollection<Connection> Connections { get; set; } = new List<Connection>();
	}
}