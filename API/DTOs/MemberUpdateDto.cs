namespace API.DTOs
{
    /// <summary>
    /// MemberUpdateDto is used when a users update their profiles
    /// </summary>
    public class MemberUpdateDto
    {
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}