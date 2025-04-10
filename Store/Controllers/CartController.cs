using Microsoft.AspNetCore.Mvc;

namespace Store.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, bool redirectToCheckout = false)
        {
            // Add to cart logic
            if (redirectToCheckout)
                return RedirectToAction("Checkout", "Cart");
            return Json(new { success = true });
        }

    }
}
