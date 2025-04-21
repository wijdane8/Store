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

var builder = WebApplication.CreateBuilder(args);

// 1. تكوين سياق قاعدة البيانات لـ Identity (ApplicationDbContext)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // اتصال قاعدة بيانات Identity

// 1.1. تكوين سياق قاعدة البيانات لنماذج المتجر (MyStoreContext)
builder.Services.AddDbContext<MyStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StoreConnection"))); // قد يكون اتصال مختلف

// 2. تكوين ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // إعدادات كلمة المرور (عدلها حسب حاجتك)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // إعدادات القفل (عدلها حسب حاجتك)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // إعدادات المستخدم
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // إعدادات تسجيل الدخول
    options.SignIn.RequireConfirmedAccount = true; // احتفظ بهذا لتأكيد البريد الإلكتروني
    options.SignIn.RequireConfirmedEmail = true;      // تأكد من تأكيد البريد الإلكتروني
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>() // اربط Identity بـ ApplicationDbContext
    .AddDefaultTokenProviders();

// 3. تكوين المصادقة والتخويل (إذا لزم الأمر بخلاف Identity الأساسي)
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options => /* خيارات JWT Bearer */);
//
// builder.Services.AddAuthorization(options =>
// {
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
// });

// 4. إضافة MVC و Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 5. إضافة الخدمات المخصصة
builder.Services.AddScoped<IEmailService, EmailService>();

// 6. تكوين الإعدادات ذات الأنواع القوية
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// 7. إضافة التسجيل (Logging)
builder.Logging.AddLog4Net(builder.Configuration.GetSection("Logging")); // الطريقة الموصى بها لتكوين Log4Net

// 8. بناء التطبيق
var app = builder.Build();

// 9. تكوين مسار معالجة طلبات HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // معلومات خطأ أكثر تفصيلاً في بيئة التطوير
    // app.UseMigrationsEndPoint(); // مطلوب فقط إذا كنت تستخدم واجهة ترحيلات EF بنشاط
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // تفعيل وسيطة المصادقة
app.UseAuthorization();   // تفعيل وسيطة التخويل


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "products",
    pattern: "Products/{action=Index}/{id?}");

app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();