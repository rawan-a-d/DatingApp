namespace API.DTOs
{
    /// <summary>
    /// Returned when the user logs in or registers
    /// </summary>
    public class UserDto
    {
        public string Username { get; set; }
        
        public string Token { get; set; }
    }
}