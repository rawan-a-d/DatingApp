using System.Linq;
using API.DTOs;
using API.Entities;
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
					));
			CreateMap<Photo, PhotoDto>();
		}
	}
}