// File: Store/Controllers/OrdersController.cs (You might need to create this controller)
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
    [Authorize] // Only logged-in users can place orders
    public class OrdersController : Controller
    {
        private readonly MyStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(MyStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // This is a simplified example of a POST endpoint that would finalize an order
        // In a real application, this would involve payment gateway integration, address selection, etc.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Or return Json({ success: false, message: "User not logged in." });
            }

            // 1. Fetch the user's active cart
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                         .ThenInclude(ci => ci.Product) // Include product to get stock info
                                     .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);

            if (cart == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Your cart is empty or invalid." });
            }

            // 2. Perform final stock availability check and reduce stock
            var newOrder = new Order // Assuming you have an Order model
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending, // Initial status, will be set to Completed after payment
                TotalAmount = 0 // Will calculate based on items
            };
            _context.Orders.Add(newOrder);

            decimal orderTotal = 0;
            foreach (var cartItem in cart.CartItems)
            {
                var product = cartItem.Product; // Product already loaded via .Include

                // Critical: Re-check stock just before finalizing the order
                if (!product.StockQuantity.HasValue || product.StockQuantity.Value < cartItem.Quantity)
                {
                    // If stock is insufficient, revert and inform user
                    // You might need to add transaction management here to revert previous changes
                    return Json(new { success = false, message = $"Insufficient stock for {product.Name}. Available: {product.StockQuantity ?? 0}" });
                }

                // Reduce stock quantity
                if (product.StockQuantity.HasValue)
                {
                    product.StockQuantity -= cartItem.Quantity;
                }

                // Create an OrderItem from the CartItem
                newOrder.OrderItems.Add(new OrderItem // Assuming OrderItem model
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price, // Use the price from the cart item
                });
                orderTotal += cartItem.Quantity * cartItem.Price;
            }
            newOrder.TotalAmount = orderTotal;

            // 3. Process Payment (Placeholder)
            // In a real system, you would integrate with a payment gateway here (Stripe, PayPal, etc.)
            bool paymentSuccessful = true; // Replace with actual payment gateway call

            if (paymentSuccessful)
            {
                newOrder.Status = OrderStatus.Completed; // Mark order as completed
                cart.Status = CartStatus.Completed; // Mark cart as completed
                _context.Carts.Remove(cart); // Optionally remove the cart or keep it marked as completed.
                                            // Removing is common for completed carts to keep the Carts table clean.

                await _context.SaveChangesAsync(); // Save all changes: new order, reduced stock, updated cart status

                return Json(new { success = true, message = "Order placed successfully!", orderId = newOrder.Id });
            }
            else
            {
                // Payment failed, revert stock changes (if any were done in a transaction)
                // This would require more advanced transaction management (e.g., using ambient transactions)
                // or marking the order as 'PaymentFailed' and manually handling stock.
                return Json(new { success = false, message = "Payment failed. Please try again." });
            }
        }

        // You might have other actions like showing order history etc.
    }
}