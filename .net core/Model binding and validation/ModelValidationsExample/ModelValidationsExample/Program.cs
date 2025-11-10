using ModelValidationsExample.CustomModelBinder;

namespace ModelValidationsExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRouting();
            builder.Services.AddControllers();
            //builder.Services.AddControllers(options =>
            //{
            //    options.ModelBinderProviders.Insert(0, new PersonBinderProvider());
            //    //you have to use this binder against built in binder thats why index 0 
            //});

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
