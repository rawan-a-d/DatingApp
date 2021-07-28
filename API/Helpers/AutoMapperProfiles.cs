using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
	/// <summary>
	/// Maps from one object to another
	/// </summary>
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			// map from, map to
			CreateMap<AppUser, MemberDto>()
				// map individual property (PhotoUrl)
				.ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
					src.Photos.FirstOrDefault(x =>
						x.IsMain).Url
					))
				// Make query more efficient by getting the age here
				.ForMember(dest => dest.Age, opt => 
					opt.MapFrom(src => 
						src.DateOfBirth.CalculateAge())
					);
			CreateMap<Photo, PhotoDto>();

			// For updating
			// from MemberUpdateDto to AppUser
			CreateMap<MemberUpdateDto, AppUser>();

			// For registering
			CreateMap<RegisterDto, AppUser>();
		}
	}
}