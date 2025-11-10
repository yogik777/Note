using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;

namespace CRUDExample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddControllersWithViews();

			//add services into IoC container 
			builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonsService, PersonsService>();
			builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
			builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

            if (builder.Environment.IsEnvironment("Test") == false)
				builder.Services.AddDbContext<ApplicationDbContext>(options =>
				{
					options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
				});

            var app = builder.Build();

			if (builder.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			if(builder.Environment.IsEnvironment("Test") == false)
				Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

			app.UseStaticFiles();
			app.UseRouting();
			app.MapControllers();

			app.Run();
		}
	}

	//public partial class Program { }  // for top level statements make the auto-generated Program accessible programmatically
}
