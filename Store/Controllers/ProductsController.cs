﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq; 
using System; 
using System.Threading.Tasks; 

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

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Cat)
                .Where(p => p.IsActive) 
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Cat)
                .Include(p => p.ProductReviews)
                    .ThenInclude(pr => pr.User) 
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            product.AverageRating = product.ProductReviews?.Any() == true ?
                product.ProductReviews.Average(r => r.Rating) : 0;
            product.ReviewCount = product.ProductReviews?.Count ?? 0;

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                product.UserHasPurchased = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .AnyAsync(oi => oi.ProductId == id &&
                                    oi.Order.UserId == userId &&
                                    oi.Order.Status == OrderStatus.Completed); 
            }
            else
            {
                product.UserHasPurchased = false;
            }

            return View(product);
        }
        [HttpGet]
        public IActionResult GetDetails(int id)
        {
            var product = _context.Products
                .Include(p => p.Cat)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    image = p.ProductImages.FirstOrDefault().ImageUrl ?? "/images/no-image.png",
                    name = p.Name,
                    shortDescription = p.ShortDescription,
                    description = p.Description,
                    price = p.Price,
                    oldPrice = p.OldPrice,
                    photo = p.Photo ?? "/images/no-image.png",
                    stockQuantity = p.StockQuantity,
                    brand = p.Brand,
                    weight = p.Weight,
                    dimensions = p.Dimensions,
                    color = p.Color,
                    material = p.Material,
                    warranty = p.Warranty,
                    averageRating = p.AverageRating,
                    reviewCount = p.ReviewCount,
                    isAvailable = p.IsAvailable,
                    categoryName = p.Cat != null ? p.Cat.Name : "غير محدد"
                })
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            return Json(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Json(new { success = false, message = "الرجاء تسجيل الدخول لإضافة المنتجات إلى السلة." });
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null || !product.IsActive)
            {
                return Json(new { success = false, message = "المنتج غير موجود أو غير متاح." });
            }

            if (!product.StockQuantity.HasValue || product.StockQuantity.Value < request.Quantity)
            {
                return Json(new
                {
                    success = false,
                    message = $"الكمية المطلوبة ({request.Quantity}) غير متوفرة. الكمية المتاحة: {product.StockQuantity ?? 0}"
                });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "فشل تحديد هوية المستخدم." });
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active); 

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Status = CartStatus.Active };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); 
            }

            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem != null)
            {
                if (product.StockQuantity.HasValue &&
                    (cartItem.Quantity + request.Quantity) > product.StockQuantity.Value)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"لا يمكن إضافة المزيد. إجمالي الكمية ({cartItem.Quantity + request.Quantity}) سيتجاوز المخزون المتاح ({product.StockQuantity.Value})."
                    });
                }
                cartItem.Quantity += request.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Price = product.Price,
                    CartId = cart.Id
                };
                cart.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync(); 

            return Json(new
            {
                success = true,
                message = "تمت إضافة المنتج إلى السلة بنجاح!",
                cartItemCount = cart.CartItems.Sum(i => i.Quantity)
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "بيانات غير صالحة." });
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null || !product.IsActive)
            {
                return Json(new { success = false, message = "المنتج غير موجود أو غير متاح." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "فشل تحديد هوية المستخدم." });
            }

            var existingWishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == request.ProductId);

            if (existingWishlistItem != null)
            {
                return Json(new { success = false, message = "المنتج موجود بالفعل في قائمة الرغبات." });
            }

            _context.Wishlists.Add(new Wishlist
            {
                UserId = userId,
                ProductId = request.ProductId,
                AddedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تمت إضافة المنتج إلى قائمة الرغبات." });
        }

        public class AddToWishlistRequest
        {
            public int ProductId { get; set; }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "فشل تحديد هوية المستخدم." });
            }

            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem == null)
            {
                return Json(new { success = false, message = "المنتج غير موجود في قائمة الرغبات." });
            }

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تمت إزالة المنتج من قائمة الرغبات." });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(int productId, int rating, string title, string comment)
        {
            if (rating < 1 || rating > 5)
            {
                return Json(new { success = false, message = "يجب أن يكون التقييم بين 1 و 5." });
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(comment))
            {
                return Json(new { success = false, message = "الرجاء تقديم العنوان والتعليق." });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsActive)
            {
                return Json(new { success = false, message = "المنتج غير موجود أو غير متاح." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "فشل تحديد هوية المستخدم." });
            }
            var hasPurchased = await _context.OrderItems
                .Include(oi => oi.Order)
                .AnyAsync(oi => oi.ProductId == productId &&
                                oi.Order.UserId == userId &&
                                oi.Order.Status == OrderStatus.Completed);

            if (!hasPurchased)
            {
                return Json(new { success = false, message = "يجب عليك شراء هذا المنتج قبل تقديم تقييم." });
            }

            var existingReview = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            if (existingReview != null)
            {
                return Json(new { success = false, message = "لقد قمت بتقييم هذا المنتج بالفعل." });
            }

            _context.ProductReviews.Add(new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Title = title,
                Comment = comment,
                ReviewDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var updatedProduct = await _context.Products
                .Include(p => p.ProductReviews)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (updatedProduct != null)
            {
                updatedProduct.AverageRating = updatedProduct.ProductReviews?.Any() == true ?
                                                updatedProduct.ProductReviews.Average(r => r.Rating) : 0;
                updatedProduct.ReviewCount = updatedProduct.ProductReviews?.Count ?? 0;
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "تم إرسال التقييم بنجاح!" });
        }

        public class AddToCartRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
