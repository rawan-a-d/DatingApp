using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	/// <summary>
	/// This class will access the bridge between our code and the db
	/// It inherits from DbContext, a class from Entity framework
	/// </summary>
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}
		
		/// <summary>
		/// Represents a table in the db called Users
		/// </summary>
		/// <value></value>
		public DbSet<AppUser> Users { get; set; }
	}
}