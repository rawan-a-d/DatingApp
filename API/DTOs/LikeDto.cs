namespace API.DTOs
{
    /// <summary>
    /// LikeDto is used to get the information about people who liked a user
    /// </summary>
    public class LikeDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
    }
}