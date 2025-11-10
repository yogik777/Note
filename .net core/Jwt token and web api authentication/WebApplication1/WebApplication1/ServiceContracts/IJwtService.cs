using WebApplication1.DTO;
using WebApplication1.Identity;

namespace WebApplication1.ServiceContracts
{
	public interface IJwtService
	{
		AuthenticationResponse CreateJwtToken(ApplicationUser user);
	}
}
