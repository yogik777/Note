using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Identity
{
	public class ApplicationUser:IdentityUser<Guid>
	{
		public string? PersonName { get; set; }

	}
}
