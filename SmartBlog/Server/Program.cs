using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Stores;
using SmartBlog.Infrastructure.Extentions;
using SmartBlog.Shared;

namespace SmartBlog.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        builder.Services.AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.UseIdentitySeeding<IdentityUser, IdentityRole>(seeds =>
        {
            seeds
                .AddRole(role: new IdentityRole(Constants.Owner))
                .AddRole(role: new IdentityRole(Constants.User))
                .AddUser(user: new() { Email = "owner@blog.com", UserName = "Owner", EmailConfirmed = true }, password: "Owner789*", roles: new IdentityRole(Constants.Owner))
                .AddUser(user: new() { Email = "user@blog.com", UserName = "User", EmailConfirmed = true }, password: "User789*", roles: new IdentityRole(Constants.User));
        });

        app.Run();
    }
}