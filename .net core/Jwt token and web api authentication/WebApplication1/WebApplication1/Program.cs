
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication1.DbContext;
using WebApplication1.Identity;
using WebApplication1.ServiceContracts;
using WebApplication1.Services;

namespace WebApplication1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddTransient<IJwtService, JwtService>();
			builder.Services.AddControllers(options =>
			{
				//Authorization policy 
				//var policyBuilder = new AuthorizationPolicyBuilder();
				//var policy = policyBuilder.RequireAuthenticatedUser().Build();

				//options.Filters.Add(new AuthorizeFilter(policy));
			});

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
			});

			builder.Services.AddDbContext<ApplicationDbContext>(options=>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
			);


			//Identity
			builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(
				options =>
				{
					options.Password.RequiredLength = 5;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireDigit = false;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders()
				.AddUserStore<UserStore<ApplicationUser,ApplicationRole,ApplicationDbContext, Guid>>()
				.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

			//JWT 
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				//if user is not authenticated by JWT then it has to automatically redirect to cookie authentication scheme 

				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateAudience = true,
					ValidateIssuer = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidAudience = builder.Configuration["Jwt:Audience"],
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					IssuerSigningKey = new SymmetricSecurityKey(
						System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
						)

				};
			});

			//builder.Services.AddAuthorization(options =>{
			//});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI V1");
				});
			}


			//app.UseHsts();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
