using Microsoft.AspNetCore.Mvc;
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
    [Authorize] 
    public class OrdersController : Controller
    {
        private readonly MyStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(MyStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); 
            }

            
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                         .ThenInclude(ci => ci.Product) 
                                     .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);

            if (cart == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Your cart is empty or invalid." });
            }
            var newOrder = new Order 
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending, 
                TotalAmount = 0 
            };
            _context.Orders.Add(newOrder);

            decimal orderTotal = 0;
            foreach (var cartItem in cart.CartItems)
            {
                var product = cartItem.Product; 
                if (!product.StockQuantity.HasValue || product.StockQuantity.Value < cartItem.Quantity)
                {
                    return Json(new { success = false, message = $"Insufficient stock for {product.Name}. Available: {product.StockQuantity ?? 0}" });
                }

                if (product.StockQuantity.HasValue)
                {
                    product.StockQuantity -= cartItem.Quantity;
                }

                newOrder.OrderItems.Add(new OrderItem 
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price, 
                });
                orderTotal += cartItem.Quantity * cartItem.Price;
            }
            newOrder.TotalAmount = orderTotal;

            bool paymentSuccessful = true;

            if (paymentSuccessful)
            {
                newOrder.Status = OrderStatus.Completed; 
                cart.Status = CartStatus.Completed; 
                _context.Carts.Remove(cart); 

                await _context.SaveChangesAsync(); 

                return Json(new { success = true, message = "Order placed successfully!", orderId = newOrder.Id });
            }
            else
            {
                return Json(new { success = false, message = "Payment failed. Please try again." });
            }
        }

    }
}