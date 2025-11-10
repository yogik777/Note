using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
	public class LoginDTO
	{
		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Password cant be blank")]
		public string Password { get; set; } = string.Empty;
	}
}
