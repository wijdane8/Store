using Store.Models;

public class CartViewModel
{
    public ICollection<CartItem>? CartItems { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }
}