using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pronia.DAL;

namespace Pronia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });
            
            var app = builder.Build();

            app.MapControllerRoute(
                "default",
                "{controller=home}/{action=index}"
                
                );
            app.UseStaticFiles();
            app.Run();
        }
    }
}
