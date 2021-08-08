using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	/// <summary>
	/// This class will access the bridge between our code and the db
	/// It inherits from IdentityDbContext, a class from Entity framework package (Microsoft.AspNetCore.Identity.EntityFrameworkCore)
	/// We need to define the classes and the type of key we're using if it is not a string
	/// </summary>
	public class DataContext : IdentityDbContext<AppUser, AppRole, int, 
		IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
		IdentityRoleClaim<int>, IdentityUserToken<int>>
	{
		public DataContext(DbContextOptions options) : base(options)
		{

		}
		
		/// <summary>
		/// Represents a table in the db called Users
		/// Provided by Identity
		/// </summary>
		/// <value></value>
		//public DbSet<AppUser> Users { get; set; }

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


			// Configure AppUser, AppRole relationship
			builder.Entity<AppUser>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(u => u.User)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();

			builder.Entity<AppRole>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(u => u.Role)
				.HasForeignKey(ur => ur.RoleId)
				.IsRequired();	
		}
	}
}