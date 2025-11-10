using Identity_API.DbContext;
using Identity_API.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Identity_API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			
			// Add services to the container.
			builder.Services.AddControllersWithViews(options =>
			{
				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
			});

			builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
			//Enable Identity in this project 
			builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(
				options=>{
					options.Password.RequiredLength = 10;
					options.Password.RequireNonAlphanumeric = true;
					options.Password.RequireUppercase = true;
					options.Password.RequireLowercase = true;
					options.Password.RequireDigit = true;
					options.Password.RequiredUniqueChars = 1;
				}
			)
				.AddEntityFrameworkStores<ApplicationDbContext>()

				.AddDefaultTokenProviders()

				.AddUserStore<UserStore<ApplicationUser,
					ApplicationRole,
					ApplicationDbContext, Guid>>()

				.AddRoleStore<RoleStore<ApplicationRole, 
					ApplicationDbContext, Guid>>();

			builder.Services.AddAuthorization(options =>
			{
				var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
				options.FallbackPolicy = policy;//it applies authorization filter for all the actions methods
												//to give access to AccountsControler we can use [AllowAnonymous]
												//we can also add [AllowAnonymous] on ActionMethod so that authenticaion and authorization
												//will get bypassed for these methods only in a controller 

				options.AddPolicy("NotAuthenticated", policy =>
				{
					policy.RequireAssertion(context =>
					{
						return !context.User.Identity.IsAuthenticated;
						//if user is not authenticated then only give him access with this
						//policy
					});
				});

			});
			//but what happens if the uer is not authenticated /loged in for that we have to provide the 
			//fallback url

			builder.Services.ConfigureApplicationCookie(
				options =>
				{
					options.LoginPath = "/Account/Login";
				});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHsts(); //informces the browser to enamble https for all the request
			app.UseHttpsRedirection();//it tells browser to use HTTPS 
			app.UseStaticFiles();
			app.UseRouting();   //Identifying action method based on route
			app.UseAuthentication();//Reading Identity cookie


			app.UseAuthorization();//Validates access permissions of the user 

			app.MapControllerRoute( //Execute the filter pipeline (action + filters)
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
