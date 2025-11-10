using Identity_API.DTO;
using Identity_API.Enums;
using Identity_API.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity_API.Controllers
{
    //[AllowAnonymous]
    //[Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [HttpGet]
        [Authorize("NotAuthenticated")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
		[Authorize("NotAuthenticated")]
        //[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            //Check for validation errors
            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany
                    (temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return Ok(ViewBag.Errors);
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.Email,
                PersonName = registerDTO.PersonName,
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                if(registerDTO.UserType == Enums.UserTypeOptions.Admin)
                {
                    //Create 'Admin' role
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null );
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        {
                            Name = UserTypeOptions.Admin.ToString(),
                        };
                        _roleManager.CreateAsync(applicationRole);

                    }
                    //Add the new user into 'Admin' role 
                    await _userManager.AddToRoleAsync(user,UserTypeOptions.Admin.ToString()); 

                }
                else
                {
					//Add the new user into 'Admin' role //The User Role is the Default Role
					await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
				}
                //SignIn
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
            }
			//TO DO: Store user registration details into Identity database

            return Ok(result.Errors);
        }

        [HttpGet]
		[Authorize("NotAuthenticated")]
		public IActionResult Login()
        {

            return Ok("hi");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return Ok(errors);
            }

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false, lockoutOnFailure: false);

            if(result.Succeeded)
            {   
                ApplicationUser user = await _userManager.FindByEmailAsync(login.Email);
                if ( user != null)
                {
                    if(await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                }

                //            if(!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                //            {
                //                return LocalRedirect(ReturnUrl);
                //}
                //return RedirectToAction(nameof(PersonController.Index, "Persons")
                return Ok("Login Successful");
            }
            else
            {
                var error = "Invalid email or password";
                return Ok(error);
            }
            //if wrong info is provided then lockout the account from login 

        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Loged Out");
        }

    }


}
