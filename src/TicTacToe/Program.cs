using Microsoft.EntityFrameworkCore;
using TicTacToe.Datasource.Model;
using TicTacToe.DI;

namespace TicTacToe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddDependencies();
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error?Code=500");
                app.UseStatusCodePagesWithReExecute("/Error", "?Code={0}");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Web}/{action=Index}/{id?}"
            );


            app.Run();
        }
    }
}
