using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
	// Why use an interface in this case?
	// 1. Testing because it is easy to mock an interface -> no need to implement anything
	// 2. Best practice
	public interface ITokenService
	{
		Task<string> CreateToken(AppUser user);
	}
}