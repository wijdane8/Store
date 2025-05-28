using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using System.Security.Claims;

namespace Store.Controllers
{
    public class WishlistController : Controller
    {
        private readonly MyStoreContext _context;

        public WishlistController(MyStoreContext context)
        {
            _context = context; 
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var items = await _context.Wishlists
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductImages)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(new WishlistViewModel { Items = items });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Remove([FromBody] RemoveWishlistRequest request)
        {
            var item = await _context.Wishlists.FindAsync(request.ItemId);
            if (item == null) return Json(new { success = false, message = "العنصر غير موجود" });

            _context.Wishlists.Remove(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFromWishlist([FromBody] AddFromWishlistRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null) return Json(new { success = false, message = "المنتج غير موجود" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Status = CartStatus.Active };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = 1,
                    Price = product.Price
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public class RemoveWishlistRequest { public int ItemId { get; set; } }
        public class AddFromWishlistRequest { public int ProductId { get; set; } }
    }
}
