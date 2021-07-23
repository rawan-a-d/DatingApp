using System;
using System.Collections.Generic;

namespace API.DTOs
{
    /// <summary>
    /// MemberDto is similar to AppUser except that 
    /// 1. it doesn't have the passwords
    /// 2. make use of PhotoDto so it doesn't cause a circular reference exception
    /// </summary>
    public class MemberDto
    {
        // AutoMapper will convert AppUser to MemberDto
		// it will calculate Age from GetAge method in AppUser
		public int Id { get; set; }
		public string Username { get; set; }
		public string PhotoUrl { get; set; } // main photo
		public int Age { get; set; }
		public string KnownAs { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastActive { get; set; }
		public string Gender { get; set; }
		public string Introduction { get; set; }
		public string LookingFor { get; set; }
		public string Interests { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public ICollection<PhotoDto> Photos { get; set; }
    }
}