namespace API.Entities
{
    // Join table for liking a user
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}