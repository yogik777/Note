using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebApplication1.DTO;
using WebApplication1.Identity;
using WebApplication1.ServiceContracts;

namespace WebApplication1.Controllers
{
	[AllowAnonymous]
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly IJwtService _jwtService;

		public AccountController(
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			SignInManager<ApplicationUser> signInManager,
			IJwtService jwtService
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_jwtService = jwtService;
		}

		[HttpPost("Register")]
		public async Task<ActionResult<AuthenticationResponse>> PostRegister(RegisterDTO registerDTO)
		{
			//Validation
			if(ModelState.IsValid == false)
			{
				string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
				return Problem(errorMessage);
			}


			//Create user 
			ApplicationUser user = new ApplicationUser()
			{
				Email = registerDTO.Email,
				PhoneNumber = registerDTO.PhoneNumber,
				UserName = registerDTO.Email,
				PersonName = registerDTO.PersonName
			};

			IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
			if (result.Succeeded) { 
				//sign-in 
				await _signInManager.SignInAsync(user, isPersistent: false);
				var authenticationResponse = _jwtService.CreateJwtToken(user);
				return Ok(authenticationResponse);
			}
			else
			{
				string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
				return Problem(errorMessage);
			}
		}

		[HttpGet("IsEmailAlreadyRegistered")]
		public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
		{
			ApplicationUser? user = await _userManager.FindByEmailAsync(email);

			if(user == null)
			{
				return Ok(true);
			}
			else
			{
				return Ok(false);
			}
		}

		[HttpPost("login")]
		public async Task<ActionResult<AuthenticationResponse>> PostLogin(LoginDTO loginDTO)
		{
			//Validation
			if(ModelState.IsValid == false)
			{
				string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v=> v.Errors).Select(e=> e.ErrorMessage));
				return Problem(errorMessage);
			}

			var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password , isPersistent: false, lockoutOnFailure: false);

			if(result.Succeeded)
			{
				ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

				if(user == null)
				{
					return NoContent();
				}
				await _signInManager.SignInAsync(user, isPersistent: false);
				var authenticationResponse = _jwtService.CreateJwtToken(user);
				return Ok(authenticationResponse);

			}
			else
			{
				return Problem("Invalid email or password");
			}

		}


		[HttpPost("generate-new-jwt-token")]
		public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
		{
			if(tokenModel.Token == null)
			{
				return BadRequest();
			}

			string? jwtToken = tokenModel.Token;
			string? refreshToken = tokenModel.RefreshToken;

			
		}


		[HttpGet("Logout")]
		public async Task<IActionResult> GetLogout(LoginDTO loginDTO)
		{
			await _signInManager.SignOutAsync();
			return NoContent();
		}


		private ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
		{
			var tokenValidationParameters = new TokenValidationParameters()
			{
				ValidateAudience = true,
				ValidAudience = this._config["Jwt:Audience"]
			};

			return null;
		}
	}
}
