using Microsoft.AspNetCore.Identity;

namespace Identity_API.IdentityEntities
{
	public class ApplicationUser:IdentityUser<Guid>
	{
		public string? PersonName { get; set; }
	}
}
