using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.DTO;
using WebApplication1.Identity;
using WebApplication1.ServiceContracts;

namespace WebApplication1.Services
{
	public class JwtService : IJwtService
	{
		private readonly IConfiguration _configuration;

		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public AuthenticationResponse CreateJwtToken(ApplicationUser user)
		{
			DateTime expiration = DateTime.UtcNow.AddMinutes(60);

			Claim[] claims = new Claim[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //Subject (user id)
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //JWT unique ID 
				new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), //Issued at (date and time of token generation)
				new Claim(ClaimTypes.NameIdentifier, user.Email.ToString()), //Unique name identifier of the user (Email) 

			};

			SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
				);

			SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			JwtSecurityToken tokenGenerator = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Audience"],
				claims,
				expires: expiration,
				signingCredentials: signingCredentials
				);

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

			string token = tokenHandler.WriteToken(tokenGenerator);

			return new AuthenticationResponse() { 
				Token = token,
				Email = user.Email,
				PersonName = user.PersonName,
				Expiration = expiration
			};
		}
	}
}
