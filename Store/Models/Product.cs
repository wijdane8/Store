using System;
using System.Collections.Generic;

namespace Store.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? CatId { get; set; }

    public string? Photo { get; set; }

    public int? Stock { get; set; }

    public decimal OldPrice { get; set; }

    public string? Sku { get; set; }

    public string? ShortDescription { get; set; }

    public string? Brand { get; set; }

    public decimal? Weight { get; set; }

    public string? Dimensions { get; set; }

    public string? Color { get; set; }

    public string? Material { get; set; }

    public string? Warranty { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Cat { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductNotification> ProductNotifications { get; set; } = new List<ProductNotification>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
