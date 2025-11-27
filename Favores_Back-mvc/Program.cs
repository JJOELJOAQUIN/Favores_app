using Microsoft.EntityFrameworkCore;
using Favores_Back_mvc.Context;

namespace Favores_Back_mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Base de datos
            builder.Services.AddDbContext<FavoresDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("FavoresDBConnection")));

            // MVC
            builder.Services.AddControllersWithViews();

            // Session correctamente configurada
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // ACTIVA SESIÓN EN EL MOMENTO CORRECTO
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
