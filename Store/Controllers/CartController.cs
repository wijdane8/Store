using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Store.Controllers
{

    public class CartController : Controller
    {
        private readonly MyStoreContext _context;

        // Make sure you have this constructor
        public CartController(MyStoreContext context)
        {
            _context = context; // This should NOT be null
        }
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
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductImages)
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);

                var model = new CartViewModel
                {
                    CartItems = cart?.CartItems?.ToList() ?? new List<CartItem>(),
                    TotalPrice = cart?.CartItems.Sum(ci => ci.Quantity * ci.Price) ?? 0,
                    TotalItems = cart?.CartItems.Sum(ci => ci.Quantity) ?? 0
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Failed to load cart: " + ex.Message
                });
            }
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