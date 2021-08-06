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

		/// <summary>
		/// Represents a table in the db called Likes
		/// </summary>
		/// <value></value>
		public DbSet<UserLike> Likes { get; set; }

		/// <summary>
		/// Represents a table in the db called Messages
		/// </summary>
		/// <value></value>
		public DbSet<Message> Messages { get; set; }


		/// <summary>
		/// This method is needed to setup the many to many relationship in like table
		/// </summary>
		/// <param name="builder"></param>
		protected override void OnModelCreating(ModelBuilder builder) 
		{
			base.OnModelCreating(builder);

			// Configure UserLike relationship
			// set primary key to combination of both keys
			builder.Entity<UserLike>()
				.HasKey(k => new
				{
					k.SourceUserId,
					k.LikedUserId
				});

			// source user can like many users
			builder.Entity<UserLike>()
				.HasOne(s => s.SourceUser)
				.WithMany(l => l.LikedUsers)
				.HasForeignKey(s => s.SourceUserId)
				.OnDelete(DeleteBehavior.Cascade);

			// liked user can be liked by many users
			builder.Entity<UserLike>()
				.HasOne(s => s.LikedUser)
				.WithMany(l => l.LikedByUsers)
				.HasForeignKey(s => s.LikedUserId)
				.OnDelete(DeleteBehavior.Cascade);


			// Configure Message, AppUser relationship
			builder.Entity<Message>()
				.HasOne(u => u.Recipient)
				.WithMany(m => m.MessagesReceived)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Message>()
				.HasOne(u => u.Sender)
				.WithMany(m => m.MessagesSent)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}