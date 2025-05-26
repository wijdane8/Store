// Store/Data/Seed/DbInitializer.cs
using Store.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Store.Data.Seed
{
    public static class DbInitializer
    {
        public static void Initialize(MyStoreContext context)
        {
            // هذا السطر يمكن أن يكون مفيدًا لتطبيق الهجرات تلقائيًا عند بدء التشغيل في بيئة التطوير
            // context.Database.Migrate();

            // Seed Categories (تعبئة الفئات)
            if (!context.Categories.Any()) // إذا لم تكن هناك أي فئات
            {
                var categories = new Category[]
                {
                    new Category { Name = "إلكترونيات" },
                    new Category { Name = "ملابس رجالية" },
                    new Category { Name = "ملابس نسائية" },
                    new Category { Name = "أجهزة منزلية" },
                    new Category { Name = "كتب" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges(); // حفظ الفئات
                Console.WriteLine("Categories seeded successfully."); // رسالة تأكيد (اختياري)
            }
            else
            {
                Console.WriteLine("Categories table already contains data. Skipping seeding.");
            }

            // ************************************************************
            // الكود الخاص بالمنتجات والصور سيتم تركه معلقًا أو حذفه هنا
            // ************************************************************

            // Seed Products (معلق: لن يتم تعبئة المنتجات)
            // if (!context.Products.Any())
            // {
            //     // استرجع الفئات التي تم إنشاؤها لربط المنتجات بها
            //     var electronicsCategory = context.Categories.FirstOrDefault(c => c.Name == "إلكترونيات");
            //     var mensWearCategory = context.Categories.FirstOrDefault(c => c.Name == "ملابس رجالية");
            //     var womensWearCategory = context.Categories.FirstOrDefault(c => c.Name == "ملابس نسائية");

            //     var products = new Product[]
            //     {
            //         new Product
            //         {
            //             Name = "هاتف ذكي X",
            //             Description = "أحدث هاتف ذكي بميزات متطورة وكاميرا عالية الدقة.",
            //             Price = 2500.00m,
            //             OldPrice = 2800.00m,
            //             StockQuantity = 100,
            //             Brand = "TechCo",
            //             CatId = electronicsCategory?.Id,
            //             DateAdded = DateTime.UtcNow,
            //             IsAvailable = true,
            //             IsFeatured = true,
            //             AverageRating = 4.5,
            //             ReviewCount = 10,
            //             Sku = "PHONE-X-001",
            //             Weight = 0.2m,
            //             Dimensions = "15x7x0.8 cm",
            //             Color = "أسود",
            //             Material = "ألومنيوم",
            //             Warranty = "سنة واحدة"
            //         },
            //         // ... (بقية المنتجات)
            //     };
            //     context.Products.AddRange(products);
            //     context.SaveChanges();
            //     Console.WriteLine("Products seeded successfully.");
            // }
            // else
            // {
            //     Console.WriteLine("Products table already contains data. Skipping seeding.");
            // }

            // Seed Product Images (معلق: لن يتم تعبئة صور المنتجات)
            // if (!context.ProductImages.Any())
            // {
            //     var productsInDb = context.Products.ToList();
            //     var productImages = new List<ProductImage>();

            //     foreach (var product in productsInDb)
            //     {
            //         productImages.Add(new ProductImage
            //         {
            //             ProductId = product.Id,
            //             ImageUrl = $"/images/products/{product.Id}-main.jpg",
            //             IsMain = true
            //         });
            //     }
            //     context.ProductImages.AddRange(productImages);
            //     context.SaveChanges();
            //     Console.WriteLine("Product Images seeded successfully.");
            // }
            // else
            // {
            //     Console.WriteLine("Product Images table already contains data. Skipping seeding.");
            // }

            // يمكنك إضافة Seed Data لأي جداول أخرى تريدها هنا بنفس الطريقة
            // مع التأكد من أن هذه الجداول لا تعتمد على المنتجات إذا لم يتم تعبئة المنتجات
        }
    }
}