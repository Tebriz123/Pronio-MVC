namespace Front_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
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
