using System;
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

			// For photos
			CreateMap<Photo, PhotoDto>();
			CreateMap<Photo, PhotoForApprovalDto>()
				.ForMember(dest => dest.Username, opt => opt.MapFrom(src => 
					src.AppUser.UserName
				));

			// For updating
			// from MemberUpdateDto to AppUser
			CreateMap<MemberUpdateDto, AppUser>();

			// For registering
			CreateMap<RegisterDto, AppUser>();

			// For messages
			CreateMap<Message, MessageDto>()
				// map individual property (SenderPhotoUrl)
				.ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src =>
					src.Sender.Photos.FirstOrDefault(x =>
						x.IsMain).Url
					))
				// map individual property (RecipientPhotoUrl)
				.ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src =>
					src.Recipient.Photos.FirstOrDefault(x =>
						x.IsMain).Url
					));


			// For date
			// add Z to dates before returning them to the client
			// not needed, added in the DataContext
			//CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
		}
	}
}