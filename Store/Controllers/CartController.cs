using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using System.Security.Claims;

namespace Store.Controllers
{

    public class CartController : Controller
    {
        private readonly MyStoreContext _context;
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, bool redirectToCheckout = false)
        {
            // Add to cart logic
            if (redirectToCheckout)
                return RedirectToAction("Checkout", "Cart");
            return Json(new { success = true });
        }
        // Controllers/CartController.cs
        [Authorize]

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product) // Include product details
                        .ThenInclude(p => p.ProductImages) // And product images
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);

            // This is the line that's likely causing the CS0266 error
            // If you're trying to assign cart.CartItems directly to a List<CartItem> in a ViewModel

            var cartViewModel = new CartViewModel
            {
                // SOLUTION: Use .ToList() to convert ICollection<CartItem> to List<CartItem>
                CartItems = cart?.CartItems.ToList() ?? new List<CartItem>() // Line ~40
            };

            return View(cartViewModel);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemRequest request)
        {
            var item = await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == request.ItemId);

            if (item == null) return Json(new { success = false, message = "العنصر غير موجود" });

            if (request.Quantity > item.Product.StockQuantity)
            {
                return Json(new
                {
                    success = false,
                    message = $"الكمية المطلوبة ({request.Quantity}) غير متوفرة. الكمية المتاحة: {item.Product.StockQuantity}"
                });
            }

            item.Quantity = request.Quantity;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveCartItemRequest request)
        {
            var item = await _context.CartItems.FindAsync(request.ItemId);
            if (item == null) return Json(new { success = false, message = "العنصر غير موجود" });

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public class UpdateCartItemRequest { public int ItemId { get; set; } public int Quantity { get; set; } }
        public class RemoveCartItemRequest { public int ItemId { get; set; } }
        // Controllers/CartController.cs
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var count = await _context.Carts
                .Where(c => c.UserId == userId && c.Status == CartStatus.Active)
                .SelectMany(c => c.CartItems)
                .SumAsync(ci => ci.Quantity);

            return Json(new { count });
        }
    }
}