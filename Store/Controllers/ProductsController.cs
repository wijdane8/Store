// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Store.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Ensure this is ApplicationUser

        public ProductsController(MyStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // GET: /Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Cat)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductReviews) // تضمين التقييمات
                    .ThenInclude(pr => pr.User) // تضمين معلومات المستخدم للتقييمات
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "يجب تسجيل الدخول أولاً" });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "المنتج غير موجود" });
            }

            if (product.Stock < quantity)
            {
                return Json(new { success = false, message = "الكمية المطلوبة غير متوفرة في المخزن" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "المستخدم غير موجود" });
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, User = user };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price.Value,
                    CartId = cart.Id
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تمت إضافة المنتج إلى السلة" });
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
            var user = await _userManager.FindByIdAsync(userId); // تحميل المستخدم

            if (user == null)
            {
                return Json(new { success = false, message = "المستخدم غير موجود" });
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
                AddedDate = DateTime.Now,
                User = user // تعيين خاصية User مباشرة
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
            var user = await _userManager.FindByIdAsync(userId); // تحميل المستخدم

            if (user == null)
            {
                return Json(new { success = false, message = "المستخدم غير موجود" });
            }

            var review = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Title = title,
                Comment = comment,
                ReviewDate = DateTime.Now,
                User = user // تعيين خاصية User مباشرة
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

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

        [HttpPost]
        public IActionResult AddToCompare(int productId)
        {
            // Add to compare logic
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult NotifyProductAvailability(int productId)
        {
            // Notification logic
            return Json(new { success = true });
        }
    }
}