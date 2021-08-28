using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        // Update isn't async because it will only update 
        // the tracking status in Entity Framework to say 
        // that something has changed
		void Update(AppUser user);

		//Task<bool> SaveAllAsync();

		Task<IEnumerable<AppUser>> GetUsersAsync();

		Task<AppUser> GetUserByIdAsync(int id);

		Task<AppUser> GetUserByUsernameAsync(string username);

		//Task<IEnumerable<MemberDto>> GetMembersAsync();
		Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

		Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser);

		Task<string> GetUserGender(string username);

		Task<MemberDto> GetUserByPhotoId(int photoId);
	}
}