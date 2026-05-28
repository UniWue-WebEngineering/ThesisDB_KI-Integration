using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThesisDB.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ThesisDbContext>(options =>
    options.UseAzureSql(connectionString));

builder.Services.AddIdentity<ThesisDbUser, IdentityRole>(options => {
        // Optionale Konfigurationen hier
    })
    .AddEntityFrameworkStores<ThesisDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Demodaten nur im Development-Environment einfügen
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ThesisDbContext>();
            var userManager = services.GetRequiredService<UserManager<ThesisDbUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            
            // SeedData asynchron aufrufen
            DataSeeder.SeedDataAsync(context, userManager, roleManager).Wait();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();