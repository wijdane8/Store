using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.Models; // تأكد من وجود هذا الـ using لـ MyStoreContext و ApplicationUser
using Store.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using Store.Data.Seed; // تأكد من مسار DbInitializer الخاص بك


var builder = WebApplication.CreateBuilder(args);

// *** 1. دمج تكوين DbContext واحد لكل من Identity وجداول المتجر (MyStoreContext) ***
var connectionString = builder.Configuration.GetConnectionString("StoreConnection") ??
                       throw new InvalidOperationException("Connection string 'StoreConnection' not found.");

builder.Services.AddDbContext<MyStoreContext>(options =>
    options.UseSqlServer(connectionString));

// *** 2. تكوين ASP.NET Core Identity لـ MyStoreContext ***
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
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<MyStoreContext>() // *** هنا نشير إلى MyStoreContext ***
    .AddDefaultTokenProviders();

// 3. Add MVC and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 4. Add custom services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailSender, EmailService>();

// 5. Configure strongly typed settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// 6. Build the application
var app = builder.Build();

// *** بداية كود Seed Data (تطبيق Migrations وتغذية البيانات) ***
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MyStoreContext>();

        // *** تطبيق الهجرات تلقائيًا (مهم جداً في بيئة التطوير) ***
        // هذا سيقوم بتطبيق جميع الهجرات (Identity وجداول المتجر) في MyStoreContext
        context.Database.Migrate();

        // استدعاء ميثود تهيئة البيانات (إذا كنت تريد إضافة بيانات أولية للمتجر)
        // تأكد من أن DbInitializer.Initialize يتلقى MyStoreContext
        DbInitializer.Initialize(context);

        // إذا أردت seeding مستخدمين Identity (يتطلب UserManager و RoleManager)
        // يمكنك حقنهم في DbInitializer أو Passهما هنا إذا كانت طريقة Initialize تسمح بذلك
        // مثال (تتطلب تعديل DbInitializer لاستقبال UserManager/RoleManager):
        // var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // DbInitializer.InitializeIdentity(context, userManager, roleManager);

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB or seeding data.");
    }
}
// *** نهاية كود Seed Data ***

// 7. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();