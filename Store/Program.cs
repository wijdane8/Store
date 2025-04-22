using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.Data;
using Store.Models;
using Store.Services;
using Microsoft.AspNetCore.Identity.UI.Services; // Add this using statement

var builder = WebApplication.CreateBuilder(args);

// 1. Configure the database context for Identity (ApplicationDbContext)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Identity database connection

// 1.1. Configure the database context for store models (MyStoreContext)
builder.Services.AddDbContext<MyStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StoreConnection"))); // Could be a different connection

// 2. Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings (adjust as needed)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings (adjust as needed)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true; // Keep this for email confirmation
    options.SignIn.RequireConfirmedEmail = true;      // Ensure email is confirmed
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>() // Link Identity to ApplicationDbContext
    .AddDefaultTokenProviders();

// 3. Configure Authentication and Authorization (if needed beyond basic Identity)
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options => /* JWT Bearer options */);
//
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
// });

// 4. Add MVC and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 5. Add custom services
builder.Services.AddScoped<IEmailService, EmailService>();
// Register your custom IEmailService as the IEmailSender that Identity UI needs
builder.Services.AddTransient<IEmailSender, EmailService>();

// 6. Configure strongly typed settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// 7. Add logging
builder.Logging.AddLog4Net(builder.Configuration.GetSection("Logging")); // Recommended way to configure Log4Net

// 8. Build the application
var app = builder.Build();

// 9. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // More detailed error information in development
    // app.UseMigrationsEndPoint(); // Only needed if you actively use EF migrations UI
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();    // Enable authorization middleware

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "products",
    pattern: "Products/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();