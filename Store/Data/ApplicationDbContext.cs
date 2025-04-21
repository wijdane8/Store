using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models; // تأكد أن هذا النطاق يحتوي على فئة ApplicationUser الخاصة بك

namespace Store.Data // أو أي نطاق اسم تفضله
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // قد لا تحتاج إلى خصائص DbSet لنماذج متجرك هنا
        // إذا كان MyStoreContext هو المسؤول عنها.

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // يمكنك هنا تخصيص بناء نموذج Identity (مثل إعادة تسمية الجداول الاختيارية)
        }
    }
}