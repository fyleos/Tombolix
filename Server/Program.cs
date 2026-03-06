using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TradeUp.Server.Controllers;
using TradeUp.Server.Data;
using TradeUp.Server.Models;
using TradeUp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddAuthorizationBuilder();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DrawServerService>();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddScoped<LicenceService>();
//builder.Services.AddScoped<StateContainerService>();
builder.Services.AddScoped<SharedUserOptionService>();

builder.Services.AddScoped<DrawController>();
builder.Services.AddScoped<RolesController>();
builder.Services.AddScoped<AuthController>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
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

app.MapGroup("/identity").MapIdentityApi<ApplicationUser>();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitializeRoles(services);
}

app.Run();

async Task InitializeRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { nameof(Roles.SuperAdmin), nameof(Roles.Essentiel), nameof(Roles.Pro), nameof(Roles.Premium) };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
