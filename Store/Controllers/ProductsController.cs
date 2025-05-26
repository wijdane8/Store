// File: Store/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq; // لـ Any() و Average()

namespace Store.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(MyStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Products

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                                         .Include(p => p.ProductImages) // Eager load ProductImages
                                         .Include(p => p.Cat) // Assuming you also want to load the Category name
                                         .ToListAsync();
            return View(products);
        }

        // GET: /Products/Details/5

        public async Task<IActionResult> Details(int? id) // 'id' should be nullable if not guaranteed
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                                        .Include(p => p.ProductImages) // Eager load product images
                                        .Include(p => p.Cat)           // Eager load category
                                        .Include(p => p.ProductReviews) // Eager load product reviews
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            // تأكد من أن المستخدم مسجل الدخول قبل محاولة الوصول إلى User.Identity
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return Json(new { success = false, message = "يجب تسجيل الدخول أولاً لإضافة منتجات إلى السلة." });
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return Json(new { success = false, message = "المنتج المحدد غير موجود." });
            }

            // *** استخدام StockQuantity و التعامل مع Nullability ***
            if (!product.StockQuantity.HasValue || product.StockQuantity.Value < request.Quantity)
            {
                return Json(new { success = false, message = $"الكمية المطلوبة ({request.Quantity}) غير متوفرة في المخزون. المتوفر: {(product.StockQuantity.HasValue ? product.StockQuantity.Value : 0)}" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "لم يتم تحديد هوية المستخدم." });
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem != null)
            {
                // التأكد من عدم تجاوز المخزون عند إضافة كمية إضافية
                if (product.StockQuantity.HasValue && (cartItem.Quantity + request.Quantity) > product.StockQuantity.Value)
                {
                    return Json(new { success = false, message = $"لا يمكنك إضافة المزيد من هذا المنتج. الكمية الإجمالية ({cartItem.Quantity + request.Quantity}) ستتجاوز المخزون المتوفر ({product.StockQuantity.Value})." });
                }
                cartItem.Quantity += request.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Price = product.Price, // Price هو decimal الآن، لا حاجة لـ .Value
                    CartId = cart.Id
                };
                _context.CartItems.Add(cartItem);
            }

            // تحديث المخزون بعد الإضافة
            if (product.StockQuantity.HasValue)
            {
                product.StockQuantity -= request.Quantity;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "تمت إضافة المنتج إلى السلة بنجاح!" });
        }

        // POST: Products/AddToWishlist
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "المنتج غير موجود" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "لم يتم تحديد هوية المستخدم." });
            }

            var existingWishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingWishlistItem != null)
            {
                return Json(new { success = false, message = "المنتج موجود بالفعل في المفضلة" });
            }

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = productId,
                AddedDate = DateTime.Now
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تمت إضافة المنتج إلى المفضلة" });
        }

        // POST: Products/RemoveFromWishlist
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "لم يتم تحديد هوية المستخدم." });
            }

            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem == null)
            {
                return Json(new { success = false, message = "المنتج غير موجود في المفضلة" });
            }

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تمت إزالة المنتج من المفضلة" });
        }

        // POST: Products/SubmitReview
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(int productId, int rating, string title, string comment)
        {
            if (rating < 1 || rating > 5)
            {
                return Json(new { success = false, message = "التقييم يجب أن يكون بين 1 و 5" });
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(comment))
            {
                return Json(new { success = false, message = "الرجاء إدخال عنوان و تعليق للتقييم" });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "المنتج غير موجود" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "لم يتم تحديد هوية المستخدم." });
            }

            var review = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Title = title,
                Comment = comment,
                ReviewDate = DateTime.Now
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            // *** تحديث متوسط التقييم وعدد التقييمات ***
            var productReviews = await _context.ProductReviews.Where(pr => pr.ProductId == productId).ToListAsync();
            product.AverageRating = productReviews.Any() ? productReviews.Average(pr => pr.Rating) : 0.0;
            product.ReviewCount = productReviews.Count;
            await _context.SaveChangesAsync(); // حفظ التغييرات على المنتج

            return Json(new { success = true, message = "شكراً لتقييمك للمنتج" });
        }

        // POST: Products/NotifyMeWhenAvailable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifyMeWhenAvailable(int productId, string email)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "المنتج غير موجود" });
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "الرجاء إدخال بريد إلكتروني صحيح" });
            }

            var existingNotification = await _context.ProductNotifications
                .FirstOrDefaultAsync(n => n.ProductId == productId && n.Email == email);

            if (existingNotification != null)
            {
                return Json(new { success = false, message = "لقد قمت بالفعل بالتسجيل للإشعار لهذا المنتج" });
            }

            var notification = new ProductNotification
            {
                ProductId = productId,
                Email = email,
                NotificationDate = DateTime.Now,
                IsNotified = false
            };

            _context.ProductNotifications.Add(notification);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "سيتم إعلامك عندما يتوفر المنتج" });
        }

        // POST: Products/AddToCompare
        [HttpPost]
        public IActionResult AddToCompare(int productId)
        {
            return Json(new { success = true, message = "تمت إضافة المنتج للمقارنة (تتطلب منطقًا إضافيًا)." });
        }

        // POST: Products/NotifyProductAvailability (مكرر مع NotifyMeWhenAvailable، يمكن حذف أحدهما إذا كانا يؤديان نفس الغرض)
        [HttpPost]
        public IActionResult NotifyProductAvailability(int productId)
        {
            return Json(new { success = true, message = "طلب إشعار التوفر قيد المعالجة (تتطلب منطقًا إضافيًا)." });
        }
    }

    // كلاس داخلي لنموذج الطلب لـ AddToCart - يجب أن يكون هنا أو في ملف منفصل (عادة في مجلد ViewModels)
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}