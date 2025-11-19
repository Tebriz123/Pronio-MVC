using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            //builder.Services.AddScoped<IEmailService, EmailService>();

            //builder.Services.AddSingleton<EmailService>();
            //builder.Services.AddTransient<EmailService>();

            var app = builder.Build();

            app.MapControllerRoute(
                "default",
                "{controller=home}/{action=index}/{id?}"
                
                );
            app.UseStaticFiles();
            app.Run();
        }
    }
}
