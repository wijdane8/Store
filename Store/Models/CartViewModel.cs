// Models/CartViewModel.cs
using Store.Models;

public class CartViewModel
{
    public List<CartItem> CartItems { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }
}